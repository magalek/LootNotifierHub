using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscordWowNotifier.Browser;
using DiscordWowNotifier.Data;
using DiscordWowNotifier.Utility;

namespace DiscordWowNotifier {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SUPER_WOW_FILE_NAME = "SuperWoWhook";
        
        private const string MESSAGE_FILE_DIRECTORY_NAME = "Imports";
        private const string CONFIG_FILE_NAME = "config.json";

        private FileSystemWatcher fileWatcher;
        private Configuration configuration;
        
        private int lastId;

        private bool started;
        
        public MainWindow() {
            InitializeComponent();
            
            Logger.AddLogHandler(new ConsoleLogHandler());
            Logger.AddLogHandler(new TextBlockLogHandler(error_box));
            Logger.AddLogHandler(new FileLogHandler());
            
            quality_dropdown.ItemsSource = Enum.GetValues(typeof(ItemQuality));
            LoadConfig();
            DataContext = configuration;
            
            start_button.Click += OnStartButtonClick;
            quality_dropdown.Loaded += QualityDropdownLoaded;
        }

        private void QualityDropdownLoaded(object sender, RoutedEventArgs e)
        {
            quality_dropdown.IsDropDownOpen = true;
            quality_dropdown.Dispatcher.Invoke(() => {}, System.Windows.Threading.DispatcherPriority.Background);
            quality_dropdown.IsDropDownOpen = false;
            
            for (var i = 0; i < quality_dropdown.Items.Count; i++)
            {
                var item = (ItemQuality)quality_dropdown.Items[i];
                if (quality_dropdown.ItemContainerGenerator.ContainerFromIndex(i) is ComboBoxItem comboBox)
                {
                    comboBox.Background = Brushes.LightGray;
                    var brush = item switch
                    {
                        ItemQuality.Thrash => Brushes.Bisque,
                        ItemQuality.Common => Brushes.White,
                        ItemQuality.Uncommon => Brushes.LawnGreen,
                        ItemQuality.Rare => Brushes.Blue,
                        ItemQuality.Epic => Brushes.Purple,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    comboBox.Foreground = brush;
                    
                }
            }
        }

        private void OnComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                quality_dropdown.Foreground = quality_dropdown.SelectedItem switch
                {
                    ItemQuality.Thrash => Brushes.Bisque,
                    ItemQuality.Common => Brushes.White,
                    ItemQuality.Uncommon => Brushes.LawnGreen,
                    ItemQuality.Rare => Brushes.Blue,
                    ItemQuality.Epic => Brushes.Purple,
                    _ => Brushes.Black
                };
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Logger.Log("Closing app.");
            SaveConfig();
        }
        
        private void OnStartButtonClick(object sender, RoutedEventArgs e) {
            started = !started;
            if (started)
            {
                if (!CheckForSuperWowPatch())
                {
                    started = false;
                    Logger.Log("No Super Wow patch found.");
                    return;
                }
                
                var result = AttachFileWatcher();
                if (!result) started = false;
            }
            else
            {
                fileWatcher.Changed -= OnMessageFileChanged;
                fileWatcher = null;
            }
            start_button.Content = started ? "Stop" : "Start";
        }

        private bool CheckForSuperWowPatch()
        {
            var files = Directory.GetFiles(configuration.GamePath, "SuperWoWhook*", SearchOption.TopDirectoryOnly);
            return files.Length != 0;
        }

        private void LoadConfig()
        {
            var configPath = Path.Combine(Environment.CurrentDirectory, CONFIG_FILE_NAME);

            if (!File.Exists(configPath))
            {
                File.Create(configPath).Close();
                File.WriteAllText(configPath, JsonSerializer.Serialize(new Configuration()));
                Logger.Log("Creating new configuration.");
                return;
            }

            configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(configPath));
        }

        private void SaveConfig()
        {
            var configPath = Path.Combine(Environment.CurrentDirectory, CONFIG_FILE_NAME);
            File.WriteAllText(configPath, JsonSerializer.Serialize(configuration));
        }
        
        private bool AttachFileWatcher()
        {
            var path = Path.Combine(configuration.GamePath, MESSAGE_FILE_DIRECTORY_NAME);
            if (!Path.Exists(path))
            {
                Logger.Log($"Could not find file \"{path}\"");
                return false;
            }
            fileWatcher = new FileSystemWatcher(path);
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Changed += OnMessageFileChanged;
            return true;
        }

        private async void OnMessageFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                string wowFileContent;
                try
                {
                    wowFileContent = await ReadWowMessage(e);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception.Message);
                    await Task.Delay(100);
                    wowFileContent = await ReadWowMessage(e);
                }
                
                
                Logger.Log($"Message content: {wowFileContent}");
                var wowResponse = JsonSerializer.Deserialize<WowResponse>(wowFileContent);
        
                if (wowResponse.CompareIds(lastId)) return;
                lastId = wowResponse.response_id;
                
                
                if ((int)wowResponse.quality < (int)configuration.QualityFilter) return;

                var ahPrice = await SeleniumUtility.GetItemInfo(wowResponse.id);
                
                var request = new DiscordMessageRequest(configuration)
                {
                    nickname = wowResponse.nick,
                    ahPrice = new Currency(ahPrice)
                };

                await request.SendAsync();
            }
            catch (Exception exception)
            {
                Logger.Log(exception.Message);
            }
        }

        private static async Task<string> ReadWowMessage(FileSystemEventArgs e)
        {
            await using var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
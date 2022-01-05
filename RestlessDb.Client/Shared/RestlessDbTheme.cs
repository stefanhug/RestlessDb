using MudBlazor;

namespace RestlessDb.Client.Shared
{
    public class RestlessDbTheme : MudTheme
    {
        public RestlessDbTheme()
        {
            Typography.H1.FontSize = "4rem";
            Typography.H2.FontSize = "3rem";
            lightPalette = Palette;
            darkPalette = new Palette()
            {
                Black = "#27282f",
                Background = "#32343d",
                BackgroundGrey = "#27282f",
                Surface = "#373840",
                TextPrimary = "#ffffffb2",
                TextSecondary = "rgba(255,255,255, 0.50)",
                DrawerBackground = "#32343d",
                DrawerText = "#ffffffb2"
            };
        }

        public bool IsDarkMode
        {
            get { return isDarkMode; }
            set 
            { 
                isDarkMode = value;
                Palette = isDarkMode ? darkPalette : lightPalette; 
            }
        }

        private bool isDarkMode = false;
        private Palette darkPalette;
        private Palette lightPalette;
    }
}

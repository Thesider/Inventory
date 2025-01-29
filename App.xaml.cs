using Inventory.Data;

namespace Inventory
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "Inventory" };
        }
        protected override async void OnStart()
        {
            var databaseService = new DatabaseService();
            var seedData = new SeedData(databaseService);
            await databaseService.InitializeAsync();
            await seedData.SeedDatabaseAsync();

        }

    }
}

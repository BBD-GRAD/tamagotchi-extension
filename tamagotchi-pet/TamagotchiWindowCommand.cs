using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using EnvDTE;
using Task = System.Threading.Tasks.Task;
using tamagotchi_pet.Utils;

namespace tamagotchi_pet
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class TamagotchiWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4472b0a0-5265-4f64-99f1-2583eb11943d");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private DTE _dte;
        private DocumentEvents _documentEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="TamagotchiWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private TamagotchiWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TamagotchiWindowCommand Instance
        {
            get;
            private set;
        }

        private async Task InitializeDTEAndEventsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            if (_dte == null)
            {
                throw new InvalidOperationException("Unable to get the DTE service.");
            }

            _documentEvents = _dte.Events.DocumentEvents;
            _documentEvents.DocumentSaved += DocumentSaved;
        }

        private void DocumentSaved(Document document)
        {
            if (TamagotchiWindowControl.CurrentInstance != null)
            {
                _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await TamagotchiWindowControl.CurrentInstance.SaveGameStateAsync();
                });
            }
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in TamagotchiWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new TamagotchiWindowCommand(package, commandService);
            await Instance.InitializeDTEAndEventsAsync();
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            _ = this.package.JoinableTaskFactory.RunAsync(async delegate
            {
                ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(TamagotchiWindow), 0, true, this.package.DisposalToken);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
            });
        }
    }
}
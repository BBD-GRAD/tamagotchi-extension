using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using EnvDTE;
using Task = System.Threading.Tasks.Task;
using tamagotchi_pet.Utils;

namespace tamagotchi_pet
{
    internal sealed class TamagotchiWindowCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("4472b0a0-5265-4f64-99f1-2583eb11943d");
        private readonly AsyncPackage package;
        private DTE _dte;
        private DocumentEvents _documentEvents;

        private TamagotchiWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static TamagotchiWindowCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new TamagotchiWindowCommand(package, commandService);
            await Instance.InitializeDTEAndEventsAsync();
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
            // Log or perform actions when a document is saved
            Logging.Logger.Debug("Document saved: " + document.FullName);
            // Here you can add any action to be performed when a document is saved
        }

        private void Execute(object sender, EventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async delegate
            {
                ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(TamagotchiWindow), 0, true, this.package.DisposalToken);
                if (window == null || window.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
            });
        }
    }
}
namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase content;

        private IServiceProvider serviceProvider;
        
        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.RequestAuthorization();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        private void RequestAuthorization(string? message = null)
        {
            var vm = new LoginViewModel(serviceProvider);
            vm.Message = message;
            var success = false;
            vm.DoLogin.Merge(vm.RequestSignUp)
                .TakeWhile(_ => !success)
                .Subscribe(authResult =>
                {
                    if (authResult.Success)
                    {
                        success = true;
                        // TODO switch to main screen
                        this.Content = this;
                    }
                    else if (authResult.SignUpRequested)
                    {
                        this.RunRegistration();
                    }
                    else
                    {
                        vm.Message = authResult.Message;
                    }
                });

            Content = vm;
        }

        private void RunRegistration()
        {
            var vm = new SignUpViewModel(this.serviceProvider);
            var registrationIsDone = false;
            vm.Register
                .Merge(vm.Cancel)
                .TakeWhile(_ => !registrationIsDone)
                .Subscribe(result =>
                {
                    if (result.Success || result.Cancelled)
                    {
                        registrationIsDone = true;
                        this.RequestAuthorization(result.Message);
                    }
                });
            this.Content = vm;
        }
    }
}
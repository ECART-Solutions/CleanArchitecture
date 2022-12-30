using Blazored.FluentValidation;

using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Pages.Authentication
{
    public partial class ConfirmAccount
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Parameter] public string userId { get; set; }
        [Parameter] public string code { get; set; }
        [Parameter] public bool selfRegistrated { get; set; }

        public ChangePasswordRequest changePasswordRequest { get; set; }

        private bool _loading = true;
        private string message = "Loading...";
        private Color color;
        private bool _actionSucceeded { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            try
            {
                await _authenticationManager.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }                  
            if (selfRegistrated)
            {
                await ConfirmationAsync(selfRegistrated);
                _actionSucceeded = true;
            }
            else
            {
                _passwordModel.Password = ApplicationConstants.DefaultPass.Pass;
            }

            _loading = false;
        }
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly ChangePasswordRequest _passwordModel = new();


        private bool _currentPasswordVisibility;


        private bool _newPasswordVisibility;
        private InputType _newPasswordInput = InputType.Password;
        private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility(bool newPassword)
        {
            if (newPassword)
            {
                if (_newPasswordVisibility)
                {
                    _newPasswordVisibility = false;
                    _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _newPasswordInput = InputType.Password;
                }
                else
                {
                    _newPasswordVisibility = true;
                    _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _newPasswordInput = InputType.Text;
                }
            }

        }

        private async Task ChangePasswordAsync()
        {
            ChangeUserPasswordRequest changeUserPasswordRequest = new ChangeUserPasswordRequest()
            {
                Password = _passwordModel.Password,
                NewPassword = _passwordModel.NewPassword,
                ConfirmNewPassword = _passwordModel.ConfirmNewPassword,
                UserId = userId
            };

            var response = await _accountManager.ChangeUserPasswordAsync(changeUserPasswordRequest);
            if (response.Succeeded)
            {
                _passwordModel.Password = string.Empty;
                _passwordModel.NewPassword = string.Empty;
                _passwordModel.ConfirmNewPassword = string.Empty;
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private async Task ConfirmationAsync(bool selfRegistration = false)
        {
            var result = await _userManager.ConfirmAccount(userId, code);
            if (result.Succeeded)
            {
                color = Color.Success;
                if (!selfRegistrated)
                {
                    await ChangePasswordAsync();
                }
            }
            else
            {
                color = Color.Error;
            }
            message = string.Join('\n', result.Messages);
            _actionSucceeded = true;
        }
        private async Task ConfirmationAsync()
        {
            await ConfirmationAsync(false);
        }

    }
}

using Blazored.FluentValidation;

using BlazorHero.CleanArchitecture.Application.Requests.Identity;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Pages.Authentication
{
    public partial class ConfirmAccount
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Parameter] public string userId { get; set; }
        [Parameter] public string code { get; set; }

        public ChangePasswordRequest changePasswordRequest { get; set; }

        private bool _loading = true;
        private string message = "Loading...";
        private Color color;
        private bool _isAllReadyActivated { get; set; } = false;
        private bool _actionSucceeded { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            await _authenticationManager.Logout();
            var result = await _userManager.GetAsync(userId);
            if (result.Succeeded)
            {
                _isAllReadyActivated = result.Data.IsActive;
                _loading = false;
                _navigationManager.NavigateTo("/login");
            }
            _passwordModel.Password = ApplicationConstants.GudefPass.Pass;
            _loading = false;
        }
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly ChangePasswordRequest _passwordModel = new();


        private bool _currentPasswordVisibility;
        private InputType _currentPasswordInput = InputType.Password;
        private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

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
            else
            {
                if (_currentPasswordVisibility)
                {
                    _currentPasswordVisibility = false;
                    _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _currentPasswordInput = InputType.Password;
                }
                else
                {
                    _currentPasswordVisibility = true;
                    _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _currentPasswordInput = InputType.Text;
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

        private async Task ConfirmationAsync()
        {
            var result = await _userManager.ConfirmAccount(userId, code);
            if (result.Succeeded)
            {
                color = Color.Success;
                await ChangePasswordAsync();
            }
            else
            {
                color = Color.Error;
            }
            message = string.Join('\n', result.Messages);
            _actionSucceeded = true;
        }

    }
}

using Microsoft.AspNetCore.Identity;

namespace Sistema_Gestion_Inventario.Data
{
    public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new() { Code = nameof(DefaultError), Description = "Ha ocurrido un error desconocido." };
        public override IdentityError ConcurrencyFailure() => new() { Code = nameof(ConcurrencyFailure), Description = "Error de concurrencia, el registro fue modificado por otro proceso." };
        public override IdentityError PasswordMismatch() => new() { Code = nameof(PasswordMismatch), Description = "La contraseña es incorrecta." };
        public override IdentityError InvalidToken() => new() { Code = nameof(InvalidToken), Description = "Token inválido." };
        public override IdentityError LoginAlreadyAssociated() => new() { Code = nameof(LoginAlreadyAssociated), Description = "Este usuario ya tiene un inicio de sesión asociado." };
        public override IdentityError InvalidUserName(string userName) => new() { Code = nameof(InvalidUserName), Description = $"Nombre de usuario '{userName}' inválido." };
        public override IdentityError InvalidEmail(string email) => new() { Code = nameof(InvalidEmail), Description = $"El e-mail '{email}' es inválido." };
        public override IdentityError DuplicateUserName(string userName) => new() { Code = nameof(DuplicateUserName), Description = $"El usuario '{userName}' ya existe." };
        public override IdentityError DuplicateEmail(string email) => new() { Code = nameof(DuplicateEmail), Description = $"El e-mail '{email}' ya está en uso." };
        public override IdentityError InvalidRoleName(string role) => new() { Code = nameof(InvalidRoleName), Description = $"El nombre de rol '{role}' es inválido." };
        public override IdentityError DuplicateRoleName(string role) => new() { Code = nameof(DuplicateRoleName), Description = $"El rol '{role}' ya existe." };
        public override IdentityError UserAlreadyHasPassword() => new() { Code = nameof(UserAlreadyHasPassword), Description = "El usuario ya tiene una contraseña establecida." };
        public override IdentityError UserLockoutNotEnabled() => new() { Code = nameof(UserLockoutNotEnabled), Description = "El bloqueo de usuario no está habilitado." };
        public override IdentityError UserAlreadyInRole(string role) => new() { Code = nameof(UserAlreadyInRole), Description = $"El usuario ya pertenece al rol '{role}'." };
        public override IdentityError UserNotInRole(string role) => new() { Code = nameof(UserNotInRole), Description = $"El usuario no pertenece al rol '{role}'." };
        public override IdentityError PasswordTooShort(int length) => new() { Code = nameof(PasswordTooShort), Description = $"La contraseña es demasiado corta. Longitud mínima: {length}." };
        public override IdentityError PasswordRequiresNonAlphanumeric() => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe contener al menos un carácter no alfanumérico." };
        public override IdentityError PasswordRequiresDigit() => new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe contener al menos un dígito ('0'-'9')." };
        public override IdentityError PasswordRequiresLower() => new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe contener al menos una letra minúscula ('a'-'z')." };
        public override IdentityError PasswordRequiresUpper() => new() { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe contener al menos una letra mayúscula ('A'-'Z')." };
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"La contraseña debe contener al menos {uniqueChars} caracteres únicos." };
    }
}
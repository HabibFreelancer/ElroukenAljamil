using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Exceptions;
using ElroukenAljamil.Identity.Domain.Enums;
using ElroukenAljamil.Identity.Domain.Events;
using ElroukenAljamil.Identity.Domain.ValueObjects;

namespace ElroukenAljamil.Identity.Domain.Entities
{
    /// <summary>
    /// Agrégat racine représentant un utilisateur de la marketplace.
    /// Hérite de AggregateRoot pour bénéficier des domain events.
    /// </summary>
    public class ApplicationUser : AggregateRoot
    {
        public string Email { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public PhoneNumber? Phone { get; private set; }
        public Address? Address { get; private set; }
        public UserRole Role { get; private set; } = UserRole.Buyer;
        public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
        public string? AvatarUrl { get; private set; }
        public DateTime? EmailVerifiedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockedUntil { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiresAt { get; private set; }

        private ApplicationUser() { } // EF Core

        public static ApplicationUser Create(
            string email,
            string userName,
            string passwordHash,
            string firstName,
            string lastName,
            string? phoneNumber = null)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email.ToLowerInvariant().Trim(),
                UserName = userName.Trim(),
                PasswordHash = passwordHash,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PhoneNumber = phoneNumber,
                Phone = phoneNumber != null ? new PhoneNumber(phoneNumber) : null,
                Role = UserRole.Buyer,
                Status = UserStatus.PendingVerification,
                CreatedAt = DateTime.UtcNow
            };

            user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email, user.UserName));
            return user;
        }

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Vérifie l'email de l'utilisateur.
        /// </summary>
        public void VerifyEmail()
        {
            if (Status != UserStatus.PendingVerification)
                throw new InvalidOperationException("L'utilisateur n'est pas en attente de vérification.");

            EmailVerifiedAt = DateTime.UtcNow;
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserEmailVerifiedDomainEvent(Id, Email));
        }

        /// <summary>
        /// Enregistre une connexion réussie.
        /// </summary>
        public void RecordSuccessfulLogin(string refreshToken, DateTime refreshTokenExpires)
        {
            LastLoginAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            LockedUntil = null;
            RefreshToken = refreshToken;
            RefreshTokenExpiresAt = refreshTokenExpires;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Enregistre une tentative de connexion échouée.
        /// Verrouille le compte après 5 tentatives.
        /// </summary>
        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 5)
            {
                LockedUntil = DateTime.UtcNow.AddMinutes(30);
                AddDomainEvent(new UserLockedOutDomainEvent(Id, Email, LockedUntil.Value));
            }

            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Vérifie si le compte est actuellement verrouillé.
        /// </summary>
        public bool IsLockedOut => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

        /// <summary>
        /// Met à jour le profil utilisateur.
        /// </summary>
        public void UpdateProfile(string firstName, string lastName, string? phoneNumber, Address? address)
        {
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber;
            Phone = phoneNumber != null ? new PhoneNumber(phoneNumber) : null;
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Change le mot de passe.
        /// </summary>
        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            RefreshToken = null; // Invalider les sessions existantes
            RefreshTokenExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Promeut l'utilisateur au rôle vendeur.
        /// </summary>
        public void PromoteToSeller()
        {
            if (Status != UserStatus.Active)
                throw new InvalidOperationException("Seul un utilisateur actif peut devenir vendeur.");

            Role = UserRole.Seller;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserPromotedToSellerDomainEvent(Id, Email));
        }

        /// <summary>
        /// Désactive le compte utilisateur.
        /// </summary>
        public void Deactivate()
        {
            Status = UserStatus.Deactivated;
            RefreshToken = null;
            RefreshTokenExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserDeactivatedDomainEvent(Id, Email));
        }

        /// <summary>
        /// Met à jour l'avatar.
        /// </summary>
        public void UpdateAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Invalide le refresh token.
        /// </summary>
        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Vérifie si le refresh token est valide.
        /// </summary>
        public bool IsRefreshTokenValid(string token)
        {
            return RefreshToken == token &&
                   RefreshTokenExpiresAt.HasValue &&
                   RefreshTokenExpiresAt.Value > DateTime.UtcNow;
        }
    }

}
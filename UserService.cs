using System;

namespace LegacyApp
{
    public class UserService
    {
        public bool AddUser(string firName,
            string surName,
            string email,
            DateTime dateOfBirth,
            int clientId)
        {
            if (!ValidateNewUser.Validate(firName, surName, email, dateOfBirth))
                return false;

            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firName,
                Surname = surName
            };

            UpdateCreditLimit(ref user);


            if (user.HasCreditLimit && user.CreditLimit < 500)
                return false;

            UserDataAccess.AddUser(user);

            return true;
        }

        private static void UpdateCreditLimit(ref User user)
        {
            int limitMultiplier = 1;
            switch (user.Client.Name)
            {
                // Пропустить проверку лимита
                case "VeryImportantClient":
                    user.HasCreditLimit = false;
                    return;
                // Проверить лимит и удвоить его
                case "ImportantClient":
                    user.HasCreditLimit = true;
                    limitMultiplier =  2;
                    break;
                // Проверить лимит
                default:
                    user.HasCreditLimit = true;
                    break;
            }

            using var userCreditService = new UserCreditServiceClient();
            int creditLimit = userCreditService.
                GetCreditLimit(user.FirstName, user.Surname, user.DateOfBirth);

            creditLimit *= limitMultiplier;
            user.CreditLimit = creditLimit;
        }
    }
}
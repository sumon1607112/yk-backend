namespace YK.Auth.Infrastructure.Identity
{
    public static class AccountUserName
    {
        public static string Create(string e164Phone, string role)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(e164Phone);
            ArgumentException.ThrowIfNullOrWhiteSpace(role);

            return $"{e164Phone}_{role.ToLowerInvariant()}";
        }

    }
}

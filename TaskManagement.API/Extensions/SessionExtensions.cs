using System.Text.Json;

namespace TaskManagement.API.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static Guid? GetUserId(this ISession session)
        {
            var userIdString = session.GetString("UserId");
            return !string.IsNullOrEmpty(userIdString) ? Guid.Parse(userIdString) : null;
        }

        public static void SetUserId(this ISession session, Guid userId)
        {
            session.SetString("UserId", userId.ToString());
        }

        public static string GetUserEmail(this ISession session)
        {
            return session.GetString("UserEmail");
        }

        public static void SetUserEmail(this ISession session, string email)
        {
            session.SetString("UserEmail", email);
        }
    }
} 
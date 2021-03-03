namespace PasswordStore.Lib.Implementation
{
    using PasswordStore.Lib.Interfaces;

    public class UserIdentity : IUserIdentity
    {
        private long? userId;

        private string key;

        private object lockObj = new object();

        public long? GetUserId()
        {
            long? id = null;
            lock (lockObj)
            {
                id = userId;
            }

            return id;
        }

        public string GetUserKey()
        {
            string key = null;
            lock (lockObj)
            {
                key = this.key;
            }

            return key;
        }

        public void SetIdentity(long id, string key)
        {
            lock (lockObj)
            {
                this.userId = id;
                this.key = key;
            }
        }

        public void ResetIdentity()
        {
            lock (lockObj)
            {
                this.userId = null;
                this.key = null;
            }
        }
    }
}
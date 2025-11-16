namespace Code.Common.Services
{
    public class IdService : IIdService
    {
        private int _currentId;

        public int Next() => ++_currentId;
    }
}


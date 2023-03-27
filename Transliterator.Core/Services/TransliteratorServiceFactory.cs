namespace Transliterator.Core.Services
{
    public class TransliteratorServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<ITransliteratorServiceObserver> _observers = new();
        private ITransliteratorService? _currentService;

        private bool useUnbufferedTransliteratorService;
        public bool UseUnbufferedTransliteratorService 
        { 
            get { return useUnbufferedTransliteratorService; }
            set
            {
                if (value != useUnbufferedTransliteratorService)
                {
                    useUnbufferedTransliteratorService = value;
                    _currentService = null;
                }
            }
        }

        public TransliteratorServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITransliteratorService CreateTransliteratorService()
        {
            ITransliteratorService newService;

            if (useUnbufferedTransliteratorService)
                newService = _serviceProvider.GetService(typeof(UnbufferedTransliteratorService)) as ITransliteratorService; 
            else
                newService = _serviceProvider.GetService(typeof(BufferedTransliteratorService)) as ITransliteratorService;

            if (newService != _currentService)
            {
                newService.TransliterationTable = _currentService?.TransliterationTable;
                _currentService = newService;
                NotifyObservers(newService);
            }

            return _currentService;
        }

        public void AddObserver(ITransliteratorServiceObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(ITransliteratorServiceObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyObservers(ITransliteratorService newService)
        {
            foreach (var observer in _observers)
            {
                observer.OnTransliteratorServiceChanged(newService);
            }
        }
    }
}

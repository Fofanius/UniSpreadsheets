namespace UniSpreadsheets
{
    public interface IDataInjectionCallbackReceiver
    {
        void OnBeforeDataInject();
        void OnAfterDataInject();
    }
}

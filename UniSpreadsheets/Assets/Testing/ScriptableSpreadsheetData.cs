using UnityEngine;

namespace UniSpreadsheets.Testing
{
    [CreateAssetMenu(menuName = "UniSpreadsheets/Scriptable Data")]
    public class ScriptableSpreadsheetData : ScriptableObject, IDataInjectionCallbackReceiver
    {
        [SpreadsheetRange("Localization")]
        [SerializeField] private LocalizationItem[] _localizationItems;

        public void OnBeforeDataInject()
        {
            Debug.Log($"Before injection: {_localizationItems?.Length} items.", this);
        }

        public void OnAfterDataInject()
        {
            Debug.Log($"After injection: {_localizationItems?.Length} items.", this);
        }
    }
}

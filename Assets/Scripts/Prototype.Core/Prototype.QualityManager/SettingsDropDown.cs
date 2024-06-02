using Prototype;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDropDown : MonoBehaviour
{
    public GameObject[] settingsImages;
    private void Awake()
    {

    }


    void Start()
    {
     
        UpdateQualityItems(QualityManager.GetInstance().currentQuality);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            var currentSetting = QualityManager.GetInstance().currentQuality;

            switch (currentSetting)
            {
                case GameQuality.Performance:
                    currentSetting = GameQuality.Balanced;
                    break;
                case GameQuality.Balanced:
                    currentSetting = GameQuality.Max;
                    break;
                case GameQuality.Max:
                    currentSetting = GameQuality.Performance;
                    break;
                default:
                    break;
            }

            QualityManager.GetInstance().SetQuality(currentSetting);
            UpdateQualityItems(QualityManager.GetInstance().currentQuality);
        });
    }
    void UpdateQualityItems(GameQuality quality)
    {
        foreach (var item in settingsImages)
        {
            item.SetActive(false);
        }

        switch (quality)
        {
            case GameQuality.Performance:
                settingsImages[0].SetActive(true);
                break;
            case GameQuality.Balanced:
                settingsImages[0].SetActive(true);
                settingsImages[1].SetActive(true);
                break;
            case GameQuality.Max:
                settingsImages[0].SetActive(true);
                settingsImages[1].SetActive(true);
                settingsImages[2].SetActive(true);
                break;
            default:
                break;
        }
    }
}
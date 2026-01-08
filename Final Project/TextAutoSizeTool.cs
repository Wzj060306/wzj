using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class TextAutoSizeTool : MonoBehaviour
{
    [Header("自适应配置")]
    public float textContainerWidth = 400f;
    public float textContainerHeight = 300f;
    public int minFontSize = 12;
    public int maxFontSize = 24;

    private Text _nativeText;
    private TMP_Text _tmpText;
    private bool _isTMP;
    private RectTransform _rectTrans;

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _rectTrans.sizeDelta = new Vector2(textContainerWidth, textContainerHeight);
        InitTextComponent();
        SetAutoSize();
    }

    private void InitTextComponent()
    {
        _tmpText = GetComponent<TMP_Text>();
        if (_tmpText != null)
        {
            _isTMP = true;
            return;
        }

        _nativeText = GetComponent<Text>();
        if (_nativeText == null)
        {
            Debug.LogError("当前物体未挂载Text或TMP_Text组件！");
            enabled = false;
        }
    }

    private void SetAutoSize()
    {
        if (_isTMP)
        {
            _tmpText.fontSize = maxFontSize;
            _tmpText.enableAutoSizing = true;
            _tmpText.fontSizeMin = minFontSize;
            _tmpText.fontSizeMax = maxFontSize;
            _tmpText.enableWordWrapping = true;
            _tmpText.overflowMode = TextOverflowModes.Truncate;
            _tmpText.alignment = TextAlignmentOptions.TopLeft;
        }
        else
        {
            _nativeText.fontSize = maxFontSize;
            _nativeText.resizeTextForBestFit = true;
            _nativeText.resizeTextMinSize = minFontSize;
            _nativeText.resizeTextMaxSize = maxFontSize;
            _nativeText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _nativeText.verticalOverflow = VerticalWrapMode.Truncate;
            _nativeText.alignment = TextAnchor.UpperLeft;
        }
    }

    public void UpdateText(string content, bool isAppend = false)
    {
        if (_isTMP)
        {
            _tmpText.text = isAppend ? $"{_tmpText.text}\n{content}" : content;
        }
        else
        {
            _nativeText.text = isAppend ? $"{_nativeText.text}\n{content}" : content;
        }
    }

    public void ClearText()
    {
        if (_isTMP) _tmpText.text = "";
        else _nativeText.text = "";
    }
}
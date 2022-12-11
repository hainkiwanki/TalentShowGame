using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerComposer : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string equipSFX;

    public Material playerMat;

    [Header("Hands")]
    public Transform leftHandPartsParent;
    public Transform rightHandPartsParent;
    private List<GameObject> leftHandParts = new List<GameObject>();
    private List<GameObject> rightHandParts = new List<GameObject>();

    [Header("Lower Arm")]
    public Transform leftLowerArmParent;
    public Transform rightLowerArmParent;
    private List<GameObject> leftLowerArmParts = new List<GameObject>();
    private List<GameObject> rightLowerArmParts = new List<GameObject>();

    [Header("Upper Arm")]
    public Transform leftUpperArmParent;
    public Transform rightUpperArmParent;
    private List<GameObject> leftUpperArmParts = new List<GameObject>();
    private List<GameObject> rightUpperArmParts = new List<GameObject>();

    [Header("Chest")]
    public Transform torsoPartParent;
    private List<GameObject> torsoParts = new List<GameObject>();

    [Header("Pants")]
    public Transform pantsPartParent;
    private List<GameObject> pantsParts = new List<GameObject>();

    [Header("Boots")]
    public Transform leftBootPartParent;
    public Transform rightBootPartParent;
    private List<GameObject> leftBootParts = new List<GameObject>();
    private List<GameObject> rightBootParts = new List<GameObject>();

    public PlayerCustomizationData data;

    private void Awake()
    {
        GetTransformChildren(leftHandPartsParent, leftHandParts);
        GetTransformChildren(rightHandPartsParent, rightHandParts);

        GetTransformChildren(leftLowerArmParent, leftLowerArmParts);
        GetTransformChildren(rightLowerArmParent, rightLowerArmParts);

        GetTransformChildren(leftUpperArmParent, leftUpperArmParts);
        GetTransformChildren(rightUpperArmParent, rightUpperArmParts);

        GetTransformChildren(torsoPartParent, torsoParts);

        GetTransformChildren(pantsPartParent, pantsParts);

        GetTransformChildren(leftBootPartParent, leftBootParts);
        GetTransformChildren(rightBootPartParent, rightBootParts);

        UpdateParts();
        UpdateColors();
    }

    public void UpdateParts()
    {
        data = SaveData.current.playerCustomization;
        SetIndices(data.handIndex, data.lowerArmIndex, data.upperArmIndex, data.chestIndex, data.pantsIndex, data.bootsIndex, false);
    }

    public void UpdateColors()
    {
        data = SaveData.current.playerCustomization;
        foreach(var color in data.colorsPerCategory)
        {
            Color col = color.Value[data.colorsInUse[color.Key]];
            SetMaterialColor(color.Key, col);
        }
    }

    #region ArmorParts
    private void GetTransformChildren(Transform _parent, List<GameObject> _children)
    {
        if (_children == null)
            _children = new List<GameObject>();
        _children.Clear();

        foreach (Transform t in _parent)
        {
            _children.Add(t.gameObject);
            t.gameObject.SetActive(_children.Count == 1);
        }
    }

    private void SetActiveChild(int _index, List<GameObject> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].SetActive(i == _index);
        }
    }

    public void SetIndices(int _hand, int _lowerArm, int _upperArm, int _chest, int _pants, int _boots, bool _useSound = true)
    {
        SetIndex(ref data.handIndex, _hand, leftHandParts, rightHandParts, _useSound);
        SetIndex(ref data.lowerArmIndex, _lowerArm, leftLowerArmParts, rightLowerArmParts, _useSound);
        SetIndex(ref data.upperArmIndex, _upperArm, leftUpperArmParts, rightUpperArmParts, _useSound);

        SetIndex(ref data.chestIndex, _chest, torsoParts, null, _useSound);
        SetIndex(ref data.pantsIndex, _pants, pantsParts, null, _useSound);
        SetIndex(ref data.bootsIndex, _boots, leftBootParts, rightBootParts, _useSound);
    }

    private void SetIndex(ref int _index, int _i, List<GameObject> _mainParts, List<GameObject> _optionalParts = null, bool _useSound = true)
    {
        if (_useSound)
            FMODUnity.RuntimeManager.PlayOneShot(equipSFX);

        _index = _i;
        if (_index < 0) _index = _mainParts.Count - 1;
        _index %= _mainParts.Count;

        SetActiveChild(_index, _mainParts);
        if(_optionalParts != null)
            SetActiveChild(_index, _optionalParts);
    }

    public void CycleHand(int _i)
    {
        SetIndex(ref data.handIndex, _i, leftHandParts, rightHandParts);
    }

    public void CycleLowerArm(int _i)
    {
        SetIndex(ref data.lowerArmIndex, _i, leftLowerArmParts, rightLowerArmParts);
    }

    public void CycleUpperArm(int _i)
    {
        SetIndex(ref data.upperArmIndex, _i, leftUpperArmParts, rightUpperArmParts);
    }

    public void CycleTorso(int _i)
    {
        SetIndex(ref data.chestIndex, _i, torsoParts);
    }

    public void CyclePants(int _i)
    {
        SetIndex(ref data.pantsIndex, _i, pantsParts);
    }

    public void CycleBoots(int _i)
    {
        SetIndex(ref data.bootsIndex, _i, leftBootParts, rightBootParts);
    }
    #endregion

    #region ArmorColors
    public void SetMaterialColor(EColorCategory _category, Color _c)
    {
        switch (_category)
        {
            case EColorCategory.PRIMARY:
                playerMat.SetColor("_Color_Primary", _c);
                break;
            case EColorCategory.SECONDARY:
                playerMat.SetColor("_Color_Secondary", _c);
                break;
            case EColorCategory.LEATHER_PRIMARY:
                playerMat.SetColor("_Color_Leather_Primary", _c);
                break;
            case EColorCategory.LEATHER_SECONDARY:
                playerMat.SetColor("_Color_Leather_Secondary", _c);
                break;
            case EColorCategory.METAL_PRIMARY:
                playerMat.SetColor("_Color_Metal_Primary", _c);
                break;
            case EColorCategory.METAL_SECONDARY:
                playerMat.SetColor("_Color_Metal_Secondary", _c);
                break;
            case EColorCategory.METAL_DARK:
                playerMat.SetColor("_Color_Metal_Dark", _c);
                break;
            default:
                break;
        }
    }
    #endregion

    public void OnPlay()
    {
        SerializationManager.Save(SaveData.current);
        SceneLoader.Inst.LoadScene(ESceneIndices.Home);
    }
}

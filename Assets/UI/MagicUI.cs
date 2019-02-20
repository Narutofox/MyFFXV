using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MagicUI : MonoBehaviour
{
    public GameObject Ligtning;
    public GameObject Blizzard;
    public GameObject Fire;
    private readonly Color _outlineActiveEffectColor = new Color(150, 255, 0, 255);
    private readonly Color _outlineNormalEffectColor = new Color(150, 255, 0, 0);
    private IList<Magic.MagicType> _magicList;
    private int _activeMagicIndex;
    public bool canUseMagic;
    private Outline _ligtningOutline;
    private Outline _blizzardOutline;
    private Outline _fireOutline;

    private Image _ligtningImage;
    private Image _blizzardImage;
    private Image _fireImage;
    void Start()
    {
        _fireOutline = Fire.GetComponent<Outline>();
        _blizzardOutline = Blizzard.GetComponent<Outline>();
        _ligtningOutline = Ligtning.GetComponent<Outline>();
        _fireImage = Fire.GetComponentsInChildren<Image>().First(x=>x.type == Image.Type.Filled);
        _blizzardImage = Blizzard.GetComponentsInChildren<Image>().First(x => x.type == Image.Type.Filled);
        _ligtningImage = Ligtning.GetComponentsInChildren<Image>().First(x => x.type == Image.Type.Filled);

        _ligtningOutline.effectColor = _outlineActiveEffectColor;
        _blizzardOutline.effectColor = _outlineNormalEffectColor;
        _fireOutline.effectColor = _outlineNormalEffectColor;

        _magicList = new List<Magic.MagicType>
            { Magic.MagicType.Lightning, Magic.MagicType.Fire, Magic.MagicType.Blizzard};
        _activeMagicIndex = 0;
        canUseMagic = true;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (_activeMagicIndex == _magicList.Count - 1)
            {
                _activeMagicIndex = 0;
            }
            else
            {
                _activeMagicIndex++;
            }
            SetActiveMagicColor();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (_activeMagicIndex == 0)
            {
                _activeMagicIndex = _magicList.Count - 1;
            }
            else
            {
                _activeMagicIndex--;
            }

            SetActiveMagicColor();
        }
    }

    public void SetActiveMagicColor()
    {     
        switch (GetCurrentMagic())
        {
            case Magic.MagicType.Fire:
            {
                _ligtningOutline.effectColor = _outlineNormalEffectColor;
                _blizzardOutline.effectColor = _outlineNormalEffectColor;
                _fireOutline.effectColor = _outlineActiveEffectColor;
                break;
            }

            case Magic.MagicType.Blizzard:
            {
                _ligtningOutline.effectColor = _outlineNormalEffectColor;
                _blizzardOutline.effectColor = _outlineActiveEffectColor;
                _fireOutline.effectColor = _outlineNormalEffectColor;
                break;
            }
            case Magic.MagicType.Lightning:
            {
                _ligtningOutline.effectColor = _outlineActiveEffectColor;
                _blizzardOutline.effectColor = _outlineNormalEffectColor;
                _fireOutline.effectColor = _outlineNormalEffectColor;
                break;
            }
        }
    }

    internal Magic.MagicType GetCurrentMagic()
    {
        return _magicList[_activeMagicIndex];
    }

    public void DecreaseMagic()
    {
        switch (GetCurrentMagic())
        {
            case Magic.MagicType.Fire:
            {
                _fireImage.fillAmount -= 0.1f;
                break;
            }

            case Magic.MagicType.Blizzard:
            {
                _blizzardImage.fillAmount -= 0.1f;
                break;
            }
            case Magic.MagicType.Lightning:
            {
                _ligtningImage.fillAmount -= 0.1f;
                break;
            }
        }
    }

    public void IncreaseMagic(Magic.MagicType magicType)
    {
        switch (magicType)
        {
            case Magic.MagicType.Fire:
            {
                _fireImage.fillAmount += 0.1f;
                break;
            }

            case Magic.MagicType.Blizzard:
            {
                _blizzardImage.fillAmount += 0.1f;
                break;
            }
            case Magic.MagicType.Lightning:
            {
                _ligtningImage.fillAmount += 0.1f;
                break;
            }
        }
    }

    public IList<Magic.MagicType> GetMagicsThatCanBeIncreased()
    {
        IList<Magic.MagicType> magicList = new List<Magic.MagicType>();
        if (_fireImage.fillAmount < 1f)
        {
            magicList.Add(Magic.MagicType.Fire);
        }
        if (_blizzardImage.fillAmount < 1f)
        {
            magicList.Add(Magic.MagicType.Blizzard);
        }
        if (_ligtningImage.fillAmount < 1f)
        {
            magicList.Add(Magic.MagicType.Lightning);
        }
        return magicList;
    }

    public bool HasMagic()
    {
        if (!canUseMagic)
        {
            return false;
        }

        switch (GetCurrentMagic())
        {
            case Magic.MagicType.Fire:
            {
               return _fireImage.fillAmount >= 0.1f;
            }
            case Magic.MagicType.Blizzard:
            {
                return _blizzardImage.fillAmount >= 0.1f;
            }
            case Magic.MagicType.Lightning:
            {
                return _ligtningImage.fillAmount >= 0.1f;
            }
        }

        return false;
    }
}

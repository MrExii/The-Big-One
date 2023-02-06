using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveDisplay : MonoBehaviour
{
    [System.Serializable]
    private class PassiveConfig
    {
        public PassiveIndex passiveIndex;
        public AnimationClip disappearAnimation;
    }

    [SerializeField] List<GameObject> passiveSlots;
    [SerializeField] List<Image> passivesImg;
    [SerializeField] PassiveConfig[] passiveConfig;
    [SerializeField] GameObject passiveSlot;

    private void Start()
    {
        foreach (var img in passivesImg)
        {
            img.enabled = false;
        }
    }

    public IEnumerator SetPassiveSprite(PassiveIndex newPassive, PassiveIndex oldPassive, int index)
    {
        float disappearDuration = 0;

        passivesImg[index].enabled = true;

        foreach (PassiveConfig passive in passiveConfig)
        {
            if (passive.passiveIndex == oldPassive)
            {
                disappearDuration = passive.disappearAnimation.length;
            }
        }

        for (int i = 0; i < passivesImg.Count; i++)
        {
            if (i == index)
            {
                if (oldPassive != PassiveIndex.None)
                {
                    passivesImg[i].GetComponent<Animator>().SetTrigger(oldPassive.ToString() + "Disappear");

                    yield return new WaitForSeconds(disappearDuration + 0.2f);
                }
                
                passivesImg[i].GetComponent<Animator>().SetTrigger(newPassive.ToString() + "Appear");

                passivesImg[i].transform.parent.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void RemovePassiveSlot(int index)
    {
        for (int i = 0; i < passivesImg.Count; i++)
        {
            if (i == index)
            {
                passivesImg.RemoveAt(index);
                Destroy(passiveSlots[index]);
            }
        }
    }

    public void AddPassiveSlot()
    {
        GameObject passiveSlotPrefab = Instantiate(passiveSlot, transform);

        passiveSlots.Add(passiveSlotPrefab);

        Image passiveImg = passiveSlotPrefab.transform.GetChild(0).GetComponent<Image>();

        passivesImg.Add(passiveImg);

        passiveImg.enabled = false;
    }

    public List<GameObject> GetPassiveSlots()
    {
        return passiveSlots;
    }
}

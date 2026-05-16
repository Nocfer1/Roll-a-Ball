using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio
{
    public class UIButtonAudio : MonoBehaviour, ISelectHandler, IPointerEnterHandler, ISubmitHandler, IPointerClickHandler
    {
        public bool isBackButton = false;

        private int lastSubmitFrame = -1;

        public void OnSelect(BaseEventData eventData)
        {
            GameAudioManager.Instance?.PlayButtonSelect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameAudioManager.Instance?.PlayButtonSelect();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            PlaySubmitSound();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlaySubmitSound();
        }

        private void PlaySubmitSound()
        {
            if (lastSubmitFrame == Time.frameCount)
                return;

            lastSubmitFrame = Time.frameCount;

            if (isBackButton)
                GameAudioManager.Instance?.PlayButtonBack();
            else
                GameAudioManager.Instance?.PlayButtonConfirm();
        }
    }
}
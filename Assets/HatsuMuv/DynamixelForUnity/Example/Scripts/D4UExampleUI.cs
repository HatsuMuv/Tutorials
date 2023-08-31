using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HatsuMuv.DynamixelForUnity.Example
{
    public class D4UExampleUI : MonoBehaviour
    {
        [SerializeField] private RectTransform panelsParent;
        [SerializeField] private D4UExampleUIElement panelPrefab;
        [SerializeField] private List<D4UExampleUIElement> panels;
        [SerializeField] private RectTransform loadingBackground;
        [SerializeField] private RectTransform loadingRing;
        [SerializeField] private Text baudrateText;

        private bool nowWaiting = true;

        private D4UExampleController controller;
        private const string BAUD_RATE_LABEL = "BaudRate: ";


        void Start()
        {
            controller = GetComponent<D4UExampleController>();
            panels = new List<D4UExampleUIElement>();
            panelsParent.GetChild(0)?.gameObject.SetActive(false);
            StartLoadingAnimation();
        }

        void Update()
        {

            if (nowWaiting)
                return;

            if (controller.NeedWait)
            {
                StartLoadingAnimation();
                return;
            }



            if (controller.Ready)
            {
                baudrateText.text = BAUD_RATE_LABEL + controller.d4u.BaudRate;

                var motorCount = controller.motors.Count;
                for (int i = 0; i < motorCount; i++)
                {
                    if (panels.Count < i + 1)
                    {
                        panels.Add(Instantiate(panelPrefab, panelsParent));
                        panels[i].Initialize(controller);
                    }

                    panels[i].UpdateParameters(controller.motors[i]);
                }
            }
        }

        public async void RefleshAll()
        {
            await controller.RefleshData();
            for(int i = 0; i < panels.Count; i++)
            {
                Destroy(panels[i].gameObject);
            }
            panels = new List<D4UExampleUIElement>();
        }

        private void StartLoadingAnimation()
        {
            nowWaiting = true;
            loadingBackground.gameObject.SetActive(true);
            loadingRing.gameObject.SetActive(true);
            if (controller == null)
                return;
            StartCoroutine(LoadingAnimationLoop());
        }

        private IEnumerator LoadingAnimationLoop()
        {
            while (true)
            {
                loadingRing.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
                if (!controller.NeedWait)
                {
                    loadingBackground.gameObject.SetActive(false);
                    loadingRing.gameObject.SetActive(false);
                    nowWaiting = false;
                    yield break;
                }
                yield return null;
            }
        }
    }
}
using UnityEngine;
using TMPro;

namespace AbyssFirstPersonController
{
    public class RivalNameTag : MonoBehaviour
    {
        [SerializeField] private RivalRank rival;
        [SerializeField] private TMP_Text label;

        private Transform cameraTransform;

        private void Awake()
        {
            if (rival == null) Fail("RivalNameTag requires a reference to RivalRank.");
            if (label == null) Fail("RivalNameTag requires a reference to TMP_Text.");

            Camera mainCam = Camera.main;
            if (mainCam == null) Fail("RivalNameTag did not find any camera with the tag MainCamera.");

            if (!enabled) return;

            cameraTransform = mainCam.transform;
            label.enabled = false;
        }

        private void OnEnable() { RivalRank.Promoted += OnPromoted; }

        private void OnDisable() { RivalRank.Promoted -= OnPromoted; }

        private void OnPromoted(RivalRank promoted)
        {
            if (promoted != rival) return;

            label.text = $"{rival.DisplayName}\nLv. {rival.Level}";
            label.enabled = true;
        }

        private void LateUpdate()
        {
            if (!label.enabled) return;

            transform.rotation = cameraTransform.rotation;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}

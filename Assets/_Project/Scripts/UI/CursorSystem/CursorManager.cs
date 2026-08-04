using System;
using System.Collections.Generic;
using Game.Bootstrap;
using Game.Input;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [Serializable]
    public enum CursorState
    {
        Default,
        Hover
    }

    // Hand-rolled software cursor: driven from a UI RawImage instead of Cursor.SetCursor, which
    // has a platform bug where ForceSoftware cursors intermittently revert to the OS hardware
    // cursor under the Both/ForceSoftware input backend combo. Rendering our own image sidesteps
    // the OS cursor entirely.
    //TODO get rid of singletone and put into DI
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] List<CursorSettings> cursors;
        [SerializeField] RectTransform canvasRect;
        [SerializeField] RectTransform cursorRect;
        [SerializeField] RawImage cursorImage;
        [SerializeField, Range(0.1f, 2f)] float cursorScale = 1f;

        private CursorState currentState = CursorState.Default;

        public static CursorManager Instance;

        private IInputService input;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Cursor.visible = false;

            ChangeCursor(CursorState.Default);
        }

        private void Start()
        {
            input = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IInputService>();
        }

        private void Update()
        {
            if (input == null || cursorRect == null || canvasRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, input.MousePosition, null, out Vector2 localPoint);
            cursorRect.anchoredPosition = localPoint;
        }

        public void ChangeCursor(CursorState state)
        {
            currentState = state;

            foreach (var settings in cursors)
            {
                if (settings._state != state) continue;
                if (cursorImage == null || settings._texture == null) return;

                cursorImage.texture = settings._texture;
                cursorRect.sizeDelta = new Vector2(settings._texture.width, settings._texture.height) * cursorScale;
                // Pivot marks the hotspot: settings._hotspot is top-left-origin pixels (Cursor.SetCursor
                // convention), RectTransform pivot is bottom-left-origin normalized, hence the y-flip.
                cursorRect.pivot = new Vector2(
                    settings._hotspot.x / settings._texture.width,
                    1f - settings._hotspot.y / settings._texture.height);
                return;
            }
        }

        // Lets cursorScale be tuned live in the Inspector (including in Play mode) without
        // needing to trigger a state change to see the result.
        private void OnValidate()
        {
            if (Application.isPlaying && cursorRect != null) ChangeCursor(currentState);
        }
    }

    [Serializable]
    public struct CursorSettings
    {
        public CursorState _state;
        public Vector2 _hotspot;
        public Texture2D _texture;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI 
{
    [Serializable]
    public enum CursorState
    { 
        Default,
        Hover
    }

    //TODO get rid of singletone and put into DI
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] List<CursorSettings> cursors;

        public static CursorManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ChangeCursor(CursorState.Default);
        }

        public void ChangeCursor(CursorState state)
        {
            foreach (var settings in cursors)
            { 
                if (settings._state == state)
                    Cursor.SetCursor(settings._texture, settings._hotspot, CursorMode.ForceSoftware);
            }
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
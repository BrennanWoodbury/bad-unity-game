using UnityEngine;
using UnityEngine.UI;

namespace Code
{

    [RequireComponent(typeof(RectTransform))]
    public class ResponsiveLayoutGroup : MonoBehaviour
    {
        [SerializeField] private float switchWidth = 900f;
        [SerializeField] private int paddingLeft = 24;
        [SerializeField] private int paddingRight = 24;
        [SerializeField] private int paddingTop = 24;
        [SerializeField] private int paddingBottom = 24;
        [SerializeField] private float spacing = 20f;
        [SerializeField] private bool childControlWidth = true;
        [SerializeField] private bool childControlHeight = true;
        [SerializeField] private bool childForceExpandWidth = true;
        [SerializeField] private bool childForceExpandHeight = false;

        private RectOffset padding;
        private HorizontalOrVerticalLayoutGroup activeLayout;
        private bool usingHorizontal;

        public RectOffset Padding
        {
            get
            {
                EnsurePadding();
                return padding;
            }
            set
            {
                padding = value;
                if (padding != null)
                {
                    paddingLeft = padding.left;
                    paddingRight = padding.right;
                    paddingTop = padding.top;
                    paddingBottom = padding.bottom;
                }
            }
        }

        public float Spacing
        {
            get => spacing;
            set => spacing = value;
        }

        public bool ChildControlWidth
        {
            get => childControlWidth;
            set => childControlWidth = value;
        }

        public bool ChildControlHeight
        {
            get => childControlHeight;
            set => childControlHeight = value;
        }

        public bool ChildForceExpandWidth
        {
            get => childForceExpandWidth;
            set => childForceExpandWidth = value;
        }

        public bool ChildForceExpandHeight
        {
            get => childForceExpandHeight;
            set => childForceExpandHeight = value;
        }

        private void Awake()
        {
            EnsurePadding();
            UpdateLayout();
        }

        private void OnEnable()
        {
            UpdateLayout();
        }

        private void Update()
        {
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            var rectTransform = (RectTransform)transform;
            var width = rectTransform.rect.width;
            if (width <= 1f)
            {
                width = Screen.width;
            }

            var useHorizontal = width >= switchWidth;
            if (activeLayout == null || usingHorizontal != useHorizontal)
            {
                SwitchLayout(useHorizontal);
            }
        }

        private void SwitchLayout(bool useHorizontal)
        {
            if (activeLayout != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(activeLayout);
                }
                else
                {
                    DestroyImmediate(activeLayout);
                }
            }

            activeLayout = useHorizontal
                ? gameObject.AddComponent<HorizontalLayoutGroup>()
                : gameObject.AddComponent<VerticalLayoutGroup>();
            usingHorizontal = useHorizontal;
            ApplySettings(activeLayout);
        }

        private void ApplySettings(HorizontalOrVerticalLayoutGroup layout)
        {
            if (layout == null)
            {
                return;
            }

            EnsurePadding();
            layout.padding = padding;
            layout.spacing = spacing;
            layout.childControlWidth = childControlWidth;
            layout.childControlHeight = childControlHeight;
            layout.childForceExpandWidth = childForceExpandWidth;
            layout.childForceExpandHeight = childForceExpandHeight;
        }

        private void EnsurePadding()
        {
            padding ??= new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        }
    }
}

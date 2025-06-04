using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Generic.UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum EFitType
        {
            UNIFORM,
            WIDTH,
            HEIGHT,
            FIXEDROWS,
            FIXEDCOLUMNS
        }

        public EFitType FitType;

        public int Rows;
        public int Columns;

        public Vector2 CellSize;
        public Vector2 Spacing;

        public bool FitX;
        public bool FitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (FitType == EFitType.WIDTH || FitType == EFitType.HEIGHT || FitType == EFitType.UNIFORM)
            {
                FitX = true;
                FitY = true;

                float sqrRt = Mathf.Sqrt(transform.childCount);
                Rows = Mathf.CeilToInt(sqrRt);
                Columns = Mathf.CeilToInt(sqrRt);
            }

            if (FitType == EFitType.WIDTH || FitType == EFitType.FIXEDCOLUMNS)
            {
                Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
            }

            if (FitType == EFitType.HEIGHT || FitType == EFitType.FIXEDROWS)
            {
                Columns = Mathf.CeilToInt(transform.childCount / (float)Rows);
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = (parentWidth / (float)Columns) - Spacing.x - (padding.left / (float)Columns) - (padding.right / (float)Columns);
            float cellHeight = (parentHeight / (float)Rows) - Spacing.y - (padding.top / (float)Rows) - (padding.bottom / (float)Rows);

            CellSize.x = FitX ? cellWidth : CellSize.x;
            CellSize.y = FitY ? cellHeight : CellSize.y;

            int colCount, rowCount = 0;
            for (int ItChild = 0; ItChild < rectChildren.Count; ItChild++)
            {
                rowCount = ItChild / Columns;
                colCount = ItChild % Columns;

                RectTransform item = rectChildren[ItChild];

                float xPos = (CellSize.x * colCount) + (Spacing.x * colCount) + padding.left + (Spacing.x * 0.5f);
                float yPos = (CellSize.y * rowCount) + (Spacing.y * rowCount) + padding.top + (Spacing.y * 0.5f);

                SetChildAlongAxis(item, 0, xPos, CellSize.x);
                SetChildAlongAxis(item, 1, yPos, CellSize.y);
            }
        }

        #region not used
        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
        #endregion
    }
}
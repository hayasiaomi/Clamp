using System.Drawing;
using Hydra.AomiCss.Handlers;
using Hydra.AomiCss.Utils;

namespace Hydra.AomiCss.Dom
{
    /// <summary>
    /// ������ʾ�ı���CSS��������
    /// </summary>
    internal abstract class CssRect
    {
        /// <summary>
        /// ��Ӧ��CSS������
        /// </summary>
        private readonly CssBox _ownerBox;

        /// <summary>
        /// Rectangle
        /// </summary>
        private RectangleF _rect;

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        private SelectionHandler _selection;

        protected CssRect(CssBox owner)
        {
            _ownerBox = owner;
        }

        /// <summary>
        /// Gets the Box where this word belongs.
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _ownerBox; }
        }

        /// <summary>
        /// Gets or sets the bounds of the rectangle
        /// </summary>
        public RectangleF Rectangle
        {
            get { return _rect; }
            set { _rect = value; }
        }

        /// <summary>
        /// ��߾�
        /// </summary>
        public float Left
        {
            get { return _rect.X; }
            set { _rect.X = value; }
        }

        /// <summary>
        /// ���߾�
        /// </summary>
        public float Top
        {
            get { return _rect.Y; }
            set { _rect.Y = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public float Width
        {
            get { return _rect.Width; }
            set { _rect.Width = value; }
        }

        /// <summary>
        /// ȫ����
        /// </summary>
        public float FullWidth
        {
            get { return _rect.Width + ActualWordSpacing; }
        }

        /// <summary>
        /// �ո�ĳ���
        /// </summary>
        public float ActualWordSpacing
        {
            get { return (OwnerBox != null ? (HasSpaceAfter ? OwnerBox.ActualWordSpacing : 0) + (IsImage ? OwnerBox.ActualWordSpacing : 0) : 0); }
        }

        /// <summary>
        /// �߶�
        /// </summary>
        public float Height
        {
            get { return _rect.Height; }
            set { _rect.Height = value; }
        }

        /// <summary>
        /// �ұ߾�
        /// </summary>
        public float Right
        {
            get { return Rectangle.Right; }
            set { Width = value - Left; }
        }

        /// <summary>
        /// �ױ߾�
        /// </summary>
        public float Bottom
        {
            get { return Rectangle.Bottom; }
            set { Height = value - Top; }
        }

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        public SelectionHandler Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        public virtual bool HasSpaceBefore
        {
            get { return false; }
        }

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        public virtual bool HasSpaceAfter
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        public virtual Image Image
        {
            get { return null; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// Gets if the word represents an image.
        /// </summary>
        public virtual bool IsImage
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a bool indicating if this word is composed only by spaces.
        /// Spaces include tabs and line breaks
        /// </summary>
        public virtual bool IsSpaces
        {
            get { return true; }
        }

        /// <summary>
        /// Gets if the word is composed by only a line break
        /// </summary>
        public virtual bool IsLineBreak
        {
            get { return false; }
        }

        /// <summary>
        /// �ı�
        /// </summary>
        public virtual string Text
        {
            get { return null; }
        }

        /// <summary>
        /// is the word is currently selected
        /// </summary>
        public bool Selected
        {
            get { return _selection != null; }
        }

        /// <summary>
        /// the selection start index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedStartIndex
        {
            get { return _selection != null ? _selection.GetSelectingStartIndex(this) : -1; }
        }

        /// <summary>
        /// the selection end index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedEndIndexOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndIndexOffset(this) : -1; }
        }

        /// <summary>
        /// the selection start offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedStartOffset
        {
            get { return _selection != null ? _selection.GetSelectedStartOffset(this) : -1; }
        }

        /// <summary>
        /// the selection end offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedEndOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndOffset(this) : -1; }
        }

        /// <summary>
        /// Gets or sets an offset to be considered in measurements
        /// </summary>
        internal float LeftGlyphPadding
        {
            get { return OwnerBox != null ? FontsUtils.GetFontLeftPadding(OwnerBox.ActualFont) : 0; }
        }

        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, Text.Length != 1 ? "s" : string.Empty);
        }
    }
}

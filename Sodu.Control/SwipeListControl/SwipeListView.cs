﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Sodu.Control.SwipeListControl
{
    public class SwipeListView : ListView
    {
        public SwipeListView()
        {
            DefaultStyleKey = typeof(SwipeListView);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var container = new SwipeListViewItem(this);
            SetItemBinding(container, "ItemLeftContentTemplate", SwipeListViewItem.LeftContentTemplateProperty);
            SetItemBinding(container, "ItemLeftBackground", SwipeListViewItem.LeftBackgroundProperty);
            SetItemBinding(container, "ItemLeftBehavior", SwipeListViewItem.LeftBehaviorProperty);

            SetItemBinding(container, "ItemRightContentTemplate", SwipeListViewItem.RightContentTemplateProperty);
            SetItemBinding(container, "ItemRightBackground", SwipeListViewItem.RightBackgroundProperty);
            SetItemBinding(container, "ItemRightBehavior", SwipeListViewItem.RightBehaviorProperty);

            return container;
        }

        private void SetItemBinding(SwipeListViewItem item, string origin, DependencyProperty destination)
        {
            var binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(origin);

            BindingOperations.SetBinding(item, destination, binding);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var container = element as SwipeListViewItem;
            if (container != null)
            {
                container.ResetSwipe();
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        internal void RaiseItemSwipe(ItemSwipeEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeListDirection.Left:
                    RaiseSwipeCommand(ItemLeftCommand, e.SwipedItem);
                    break;
                case SwipeListDirection.Right:
                    RaiseSwipeCommand(ItemRightCommand, e.SwipedItem);
                    break;
            }

            if (ItemSwipe != null)
                ItemSwipe(this, e);
        }

        private void RaiseSwipeCommand(ICommand command, object swipedItem)
        {
            if (command == null)
            {
                return;
            }

            if (command.CanExecute(swipedItem))
            {
                command.Execute(swipedItem);
            }
        }

        /// <summary>
        /// Occurs when an item in the list view is swiped from left or right.
        /// </summary>
        public event ItemSwipeEventHandler ItemSwipe;

        #region ItemLeftContentTemplate
        public DataTemplate ItemLeftContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemLeftContentTemplateProperty); }
            set { SetValue(ItemLeftContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemLeftContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLeftContentTemplateProperty =
            DependencyProperty.Register("ItemLeftContentTemplate", typeof(DataTemplate), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion

        #region ItemLeftBackground
        public Brush ItemLeftBackground
        {
            get { return (Brush)GetValue(ItemLeftBackgroundProperty); }
            set { SetValue(ItemLeftBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemLeftBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLeftBackgroundProperty =
            DependencyProperty.Register("ItemLeftBackground", typeof(Brush), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion

        #region ItemLeftBehavior
        public SwipeListBehavior ItemLeftBehavior
        {
            get { return (SwipeListBehavior)GetValue(ItemLeftBehaviorProperty); }
            set { SetValue(ItemLeftBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemLeftBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLeftBehaviorProperty =
            DependencyProperty.Register("ItemLeftBehavior", typeof(SwipeListBehavior), typeof(SwipeListView), new PropertyMetadata(SwipeListBehavior.Collapse));
        #endregion

        #region ItemRightCommand
        public ICommand ItemLeftCommand
        {
            get { return (ICommand)GetValue(ItemLeftCommandProperty); }
            set { SetValue(ItemLeftCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemLeftCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLeftCommandProperty =
            DependencyProperty.Register("ItemLeftCommand", typeof(ICommand), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion

        #region ItemRightContentTemplate
        public DataTemplate ItemRightContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemRightContentTemplateProperty); }
            set { SetValue(ItemRightContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemRightContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRightContentTemplateProperty =
            DependencyProperty.Register("ItemRightContentTemplate", typeof(DataTemplate), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion

        #region ItemRightBackground
        public Brush ItemRightBackground
        {
            get { return (Brush)GetValue(ItemRightBackgroundProperty); }
            set { SetValue(ItemRightBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemRightBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRightBackgroundProperty =
            DependencyProperty.Register("ItemRightBackground", typeof(Brush), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion

        #region ItemRightBehavior
        public SwipeListBehavior ItemRightBehavior
        {
            get { return (SwipeListBehavior)GetValue(ItemRightBehaviorProperty); }
            set { SetValue(ItemRightBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemRightBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRightBehaviorProperty =
            DependencyProperty.Register("ItemRightBehavior", typeof(SwipeListBehavior), typeof(SwipeListView), new PropertyMetadata(SwipeListBehavior.Expand));
        #endregion

        #region ItemRightCommand
        public ICommand ItemRightCommand
        {
            get { return (ICommand)GetValue(ItemRightCommandProperty); }
            set { SetValue(ItemRightCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemRightCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRightCommandProperty =
            DependencyProperty.Register("ItemRightCommand", typeof(ICommand), typeof(SwipeListView), new PropertyMetadata(null));
        #endregion


    }
}

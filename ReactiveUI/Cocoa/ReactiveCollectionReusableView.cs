﻿using System;
using ReactiveUI;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Splat;
using System.Reactive;

namespace ReactiveUI
{
    public abstract class ReactiveCollectionReusableView : UICollectionReusableView, 
        IReactiveNotifyPropertyChanged<ReactiveCollectionReusableView>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveCollectionReusableView() : base() { setupRxObj(); }
        protected ReactiveCollectionReusableView(RectangleF frame) : base(frame) { setupRxObj(); }
        protected ReactiveCollectionReusableView(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionReusableView(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionReusableView(NSCoder coder) : base(coder) { setupRxObj(); }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) 
        {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) 
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            activated.OnNext(Unit.Default);
        }

        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            deactivated.OnNext(Unit.Default);
        }
    }
}

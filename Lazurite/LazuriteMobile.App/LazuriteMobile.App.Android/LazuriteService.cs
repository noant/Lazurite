using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace LazuriteMobile.App.Droid
{
    [Service(Exported = false)]
    public class LazuriteService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }
    }

    public class Binder : IBinder
    {
        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string InterfaceDescriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsBinderAlive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Dump(FileDescriptor fd, string[] args)
        {
            throw new NotImplementedException();
        }

        public void DumpAsync(FileDescriptor fd, string[] args)
        {
            throw new NotImplementedException();
        }

        public void LinkToDeath(IBinderDeathRecipient recipient, int flags)
        {
            throw new NotImplementedException();
        }

        public bool PingBinder()
        {
            throw new NotImplementedException();
        }

        public IInterface QueryLocalInterface(string descriptor)
        {
            throw new NotImplementedException();
        }

        public bool Transact(int code, Parcel data, Parcel reply, [GeneratedEnum] TransactionFlags flags)
        {
            throw new NotImplementedException();
        }

        public bool UnlinkToDeath(IBinderDeathRecipient recipient, int flags)
        {
            throw new NotImplementedException();
        }
    }
}
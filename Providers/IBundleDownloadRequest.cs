using System;
using UnityEngine;

namespace AssetBundleSimplified
{
    public interface IBundleDownloadRequest
    {
        bool IsDone();
        bool HasFailed();

        int ErrorCode { get; }

        event Action Completed;

        AssetBundle Bundle
        {
            get; 
        }
    }
}
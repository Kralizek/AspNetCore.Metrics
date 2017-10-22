using Amazon.Util;
using System;

namespace Kralizek.AspNetCore.Metrics
{
    public interface IAWSMetadataProvider
    {
        string GetInstanceId();

        string GetInstanceType();


        string GetAvailabilityZone();

        string GetAmiId();

        bool IsInAWS();
    }

    public static class AWSMetadataProvider
    {
        private static IAWSMetadataProvider _provider = SdkMetadataProvider.Instance;

        public static void SetInstance(IAWSMetadataProvider provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        public static void ResetInstance() => _provider = SdkMetadataProvider.Instance;

        public static string GetAmiId() => _provider.GetAmiId();

        public static string GetAvailabilityZone() => _provider.GetAvailabilityZone();

        public static string GetInstanceId() => _provider.GetInstanceId();

        public static string GetInstanceType() => _provider.GetInstanceType();

        public static bool IsInAWS() => _provider.IsInAWS();


    }

    public class SdkMetadataProvider : IAWSMetadataProvider
    {
        private SdkMetadataProvider() { }

        public static IAWSMetadataProvider Instance = new SdkMetadataProvider();

        public string GetAmiId() => EC2InstanceMetadata.AmiId;

        public string GetAvailabilityZone() => EC2InstanceMetadata.AvailabilityZone;

        public string GetInstanceId() => EC2InstanceMetadata.InstanceId;

        public string GetInstanceType() => EC2InstanceMetadata.InstanceType;

        public bool IsInAWS() => EC2InstanceMetadata.InstanceId != null;
    }
}

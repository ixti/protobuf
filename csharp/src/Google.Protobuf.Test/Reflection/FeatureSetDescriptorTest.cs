#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion

using Google.Protobuf.Reflection;
using NUnit.Framework;
using System;
using System.Linq;

namespace Google.Protobuf.Test.Reflection;

public partial class FeatureSetDescriptorTest
{
    [Test]
    [TestCase(Edition.Proto2, Edition.Legacy)]
    [TestCase(Edition.Proto3, Edition.Proto3)]
    [TestCase(Edition._2023, Edition._2023)]
    public void DefaultsMatchCanonicalSerializedForm(Edition editionToRequest,
        Edition expectDefaultsFrom)
    {
        var canonicalFeatureSetDefaults = FeatureSetDefaults.Parser
          .WithDiscardUnknownFields(true) // Discard language-specific extensions.
          .ParseFrom(Convert.FromBase64String(DefaultsBase64));

        // Find the expected FeatureSetDefaults matching the specified edition.
        // This isn't always the edition we are requesting, due to the logic of "if nothing
        // has changed since the previous edition, it isn't specified in FeatureSetDefaults".
        var editionFeatureSetDefaults = canonicalFeatureSetDefaults.Defaults
            .Single(def => def.Edition == expectDefaultsFrom);

        var expectedFeatureSet = new FeatureSet();
        expectedFeatureSet.MergeFrom(editionFeatureSetDefaults.FixedFeatures);
        expectedFeatureSet.MergeFrom(editionFeatureSetDefaults.OverridableFeatures);

        var actualFeatureSet = FeatureSetDescriptor.GetEditionDefaults(editionToRequest).Proto;

        Assert.AreEqual(expectedFeatureSet, actualFeatureSet);
    }
}

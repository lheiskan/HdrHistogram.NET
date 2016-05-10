﻿using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;

namespace HdrHistogram.UnitTests
{
    [TestFixture]
    public sealed class HgrmPercentileDistrubutionOutputTests
    {
        [Test, Combinatorial]
        public void PercentileDistrubution_hgrm_output_is_in_correct_format(
            [Values(TimeSpan.TicksPerHour)]long highestTrackableValue,
            [Values(1, 2, 3, 4, 5)]int signigicantDigits,
            [Values(10000.0)]double scaling,
            [Values(5, 10, 20)]int percentileTicksPerHalfDistance)
        {
            var fileName = $"Sample_10kBy1k_{signigicantDigits}sf_TicksPerHour_asMs_{percentileTicksPerHalfDistance}percPerHalfDistance.hgrm";
            var expected = GetEmbeddedFileText(fileName);

            var histogram = new LongHistogram(highestTrackableValue, signigicantDigits);
            LoadHistogram(histogram);

            var writer = new StringWriter();
            histogram.OutputPercentileDistribution(writer, percentileTicksPerHalfDistance, scaling);
            var actual = writer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test, Combinatorial]
        public void PercentileDistrubution_csv_output_is_in_correct_format(
            [Values(TimeSpan.TicksPerHour)]long highestTrackableValue,
            [Values(1, 2, 3, 4, 5)]int signigicantDigits,
            [Values(10000.0)]double scaling,
            [Values(5, 10, 20)]int percentileTicksPerHalfDistance)
        {
            var fileName = $"Sample_10kBy1k_{signigicantDigits}sf_TicksPerHour_asMs_{percentileTicksPerHalfDistance}percPerHalfDistance.csv";
            var expected = GetEmbeddedFileText(fileName);

            var histogram = new LongHistogram(highestTrackableValue, signigicantDigits);
            LoadHistogram(histogram);

            var writer = new StringWriter();
            histogram.OutputPercentileDistribution(writer, percentileTicksPerHalfDistance, scaling, true);
            var actual = writer.ToString();

            Assert.AreEqual(expected, actual);
        }
        
        private Stream GetEmbeddedFileStream(string filename)
        {
            var fileName = string.Format(CultureInfo.InvariantCulture, "HdrHistogram.UnitTests.Resources.{0}", filename);
            return GetType()
                .Assembly
                .GetManifestResourceStream(fileName);
        }

        private string GetEmbeddedFileText(string filename)
        {
            using (var stream = GetEmbeddedFileStream(filename))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        private static void LoadHistogram(HistogramBase histogram)
        {
            for (int i = 0; i < 10000; i += 1000)
            {
                histogram.RecordValue(i);
            }
        }
    }
}

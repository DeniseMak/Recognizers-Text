﻿using System.Collections.Immutable;
using Microsoft.Recognizers.Definitions;
using Microsoft.Recognizers.Text.DateTime.Utilities;

namespace Microsoft.Recognizers.Text.DateTime
{
    public abstract class BaseDateParserConfiguration : ICommonDateTimeParserConfiguration
    {
        public virtual IExtractor CardinalExtractor { get; protected set; }

        public virtual IExtractor IntegerExtractor { get; protected set; }

        public virtual IExtractor OrdinalExtractor { get; protected set; }

        public virtual IParser NumberParser { get; protected set; }

        public virtual IExtractor DateExtractor { get; protected set; }

        public virtual IExtractor TimeExtractor { get; protected set; }

        public virtual IExtractor DateTimeExtractor { get; protected set; }

        public virtual IExtractor DurationExtractor { get; protected set; }

        public virtual IExtractor DatePeriodExtractor { get; protected set; }

        public virtual IExtractor TimePeriodExtractor { get; protected set; }

        public virtual IExtractor DateTimePeriodExtractor { get; protected set; }

        public virtual IDateTimeParser DateParser { get; protected set; }

        public virtual IDateTimeParser TimeParser { get; protected set; }

        public virtual IDateTimeParser DateTimeParser { get; protected set; }

        public virtual IDateTimeParser DurationParser { get; protected set; }

        public virtual IDateTimeParser DatePeriodParser { get; protected set; }

        public virtual IDateTimeParser TimePeriodParser { get; protected set; }

        public virtual IDateTimeParser DateTimePeriodParser { get; protected set; }

        public virtual IImmutableDictionary<string, int> MonthOfYear { get; protected set; }

        public virtual IImmutableDictionary<string, int> Numbers { get; protected set; }

        public virtual IImmutableDictionary<string, double> DoubleNumbers { get; protected set; }

        public virtual IImmutableDictionary<string, long> UnitValueMap { get; protected set; }

        public virtual IImmutableDictionary<string, string> SeasonMap { get; protected set; }

        public virtual IImmutableDictionary<string, string> UnitMap { get; protected set; }

        public virtual IImmutableDictionary<string, int> CardinalMap { get; protected set; }

        public virtual IImmutableDictionary<string, int> DayOfWeek { get; protected set; }

        public virtual IImmutableDictionary<string, int> DayOfMonth => BaseDateTime.DayOfMonthDictionary.ToImmutableDictionary();

        public virtual IDateTimeUtilityConfiguration UtilityConfiguration { get; protected set; }
    }
}
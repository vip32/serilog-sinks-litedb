// Copyright 2014-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Serilog.Sinks.LiteDB
{
    /// <summary>
    /// This enum help specify when the database file should be closed and a new file should be created.
    /// </summary>
    public enum RollingPeriod
    {
        /// <summary>
        /// Always same file
        /// </summary>
        Never,
        /// <summary>
        /// Creates a new file each 15 minute period.
        /// </summary>
        Quarterly,
        /// <summary>
        /// Creates a new file each 30 minutes.
        /// </summary>
        HalfHour,
        /// <summary>
        /// Creates a new file each hour.
        /// </summary>
        Hourly,
        /// <summary>
        /// Creates a new file each day.
        /// </summary>
        Daily,
        /// <summary>
        /// Creates a new file each week start (sunday)
        /// </summary>
        Weekly,
        /// <summary>
        /// Creates a new file at beginning of month.
        /// </summary>
        Monthly
    }
}
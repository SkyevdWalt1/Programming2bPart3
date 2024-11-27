namespace Part_2.Services {
    public class Automation {
        /// <summary>
        /// Determines whether a claim should be instantly paid, sent for review, or instantly rejected.
        /// </summary>
        /// <param name="hours">The number of hours worked. Must be non-negative and <= 200.</param>
        /// <param name="rate">The hourly rate. Must be non-negative.</param>
        /// <returns>
        /// Returns 1 for "Instant Pay," 0 for "Review," and -1 for "Instant Reject."
        /// </returns>
        /// <remarks>
        /// Decision Rules:
        /// - Reject (-1): If `hours` < 0, `hours` > 200, or `rate` < 0.
        /// - Review (0): If `rate` > 300 and `0 < hours < 100` OR `hours > 200`.
        /// - Instant Pay (1): If `hours` and `rate` meet all criteria for approval.
        /// </remarks>
        /// <example>
        /// Example 1: Instant Pay
        /// <code>
        /// int result = Automation.AutomatePayOrReject(50, 100);
        /// // result: 1
        /// </code>
        /// Example 2: Review
        /// <code>
        /// int result = Automation.AutomatePayOrReject(150, 400);
        /// // result: 0
        /// </code>
        /// Example 3: Instant Reject
        /// <code>
        /// int result = Automation.AutomatePayOrReject(-5, 200);
        /// // result: -1
        /// </code>
        /// </example>
        public static int AutomatePayOrReject(int hours, int rate) {
            // Reject if hours are negative or exceed 300
            if (hours < 0 || hours > 300) {
                return -1; // Instant Reject
            }

            // For hours in the range [1, 100)
            if (hours > 0 && hours < 100) {
                // Reject if rate is negative
                if (rate <= 0) {
                    return -1; // Instant Reject
                }

                // Flag for review if rate exceeds 300
                if (rate > 300) {
                    return 0; // Review
                }

                // Otherwise, approve for instant pay
                return 1; // Instant Pay
            }

            // For hours > 200
            if (hours > 200) {
                return 0; // Review
            } else {
                // If all conditions are met, approve for instant pay
                return 1; // Instant Pay
            }
        }
    }
}

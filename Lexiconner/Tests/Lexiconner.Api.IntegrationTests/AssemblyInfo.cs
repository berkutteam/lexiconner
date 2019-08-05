using Xunit;

/*
 * When running tests in parallel (by default each test class run separately when running all tests)
 * VS Test Runner hangs on and we can't see results until click Cancel button.
 * Also awoid using .Result on tasks as it also can lead to freezing according to comments on StackOverflow.
 * Quick solution - run tests sequentially.
 * See more: https://xunit.net/docs/running-tests-in-parallel.html
 */
[assembly: CollectionBehavior(DisableTestParallelization = true)]


using SqlQueryBuilder.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace SqlQueryBuilderTests.Utilities
{
    [ExcludeFromCodeCoverage]
    public class EnsureTests
    {
        private string _fakeString;

        [Fact]
        public void Ensure_ArgumentNotNull_ArgumentNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNull(null, nameof(_fakeString)));
        }

        [Fact]
        public void Ensure_ArgumentNotNullOrEmptyString_ArgumentNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Ensure.ArgumentNotNullOrEmptyString(null, nameof(_fakeString)));
        }

        [Fact]
        public void Ensure_ArgumentNotNullOrEmptyString_Argument_EmptyString_Throws()
        {
            _fakeString = string.Empty;
            Assert.Throws<ArgumentException>(() => Ensure.ArgumentNotNullOrEmptyString(_fakeString, nameof(_fakeString)));
        }
    }
}

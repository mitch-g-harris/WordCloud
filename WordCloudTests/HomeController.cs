using System;
using WordCloud.Controllers;
using Xunit;

namespace WordCloudTests
{
    public class HomeControllerTests
    {

        VerbAndNounChecker verbAndNounChecker = new VerbAndNounChecker();


        [Fact]
        [Trait("Category", "Verb and Noun Checker")]
        public void ShouldReturnTrueWhenNotNounOrVerb()
        {
            Assert.True(verbAndNounChecker.NotVerbOrNoun("the"));
        }

        [Fact]
        [Trait("Category", "Verb and Noun Checker")]
        public void ShouldReturnFalseWhenNounOrVerb()
        {
            Assert.False(verbAndNounChecker.NotVerbOrNoun("stop"));
        }
    }
}

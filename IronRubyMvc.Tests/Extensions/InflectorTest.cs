using System.Collections.Generic;
using IronRubyMvc.Extensions;
using Xunit;

namespace IronRubyMvc.Tests.Extensions
{
    public class InflectorTest
    {
        #region Fixture Data

        public static readonly Dictionary<string, string> _camelWithModuleToUnderscoreWithSlash =
            new Dictionary<string, string>();

        public static readonly Dictionary<string, string> _mixtureToTitleCase = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> _ordinalNumbers = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> _pascalToUnderscore = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> _pascalToUnderscoreWithoutReverse =
            new Dictionary<string, string>();

        public static readonly Dictionary<string, string> _singularToPlural = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> _underscoresToDashes = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> _underscoreToCamel = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> _underscoreToHuman = new Dictionary<string, string>();

        static InflectorTest()
        {
            _singularToPlural.Add("search", "searches");
            _singularToPlural.Add("switch", "switches");
            _singularToPlural.Add("fix", "fixes");
            _singularToPlural.Add("box", "boxes");
            _singularToPlural.Add("process", "processes");
            _singularToPlural.Add("address", "addresses");
            _singularToPlural.Add("case", "cases");
            _singularToPlural.Add("stack", "stacks");
            _singularToPlural.Add("wish", "wishes");
            _singularToPlural.Add("fish", "fish");

            _singularToPlural.Add("category", "categories");
            _singularToPlural.Add("query", "queries");
            _singularToPlural.Add("ability", "abilities");
            _singularToPlural.Add("agency", "agencies");
            _singularToPlural.Add("movie", "movies");

            _singularToPlural.Add("archive", "archives");

            _singularToPlural.Add("index", "indices");

            _singularToPlural.Add("wife", "wives");
            _singularToPlural.Add("safe", "saves");
            _singularToPlural.Add("half", "halves");

            _singularToPlural.Add("move", "moves");

            _singularToPlural.Add("salesperson", "salespeople");
            _singularToPlural.Add("person", "people");

            _singularToPlural.Add("spokesman", "spokesmen");
            _singularToPlural.Add("man", "men");
            _singularToPlural.Add("woman", "women");

            _singularToPlural.Add("basis", "bases");
            _singularToPlural.Add("diagnosis", "diagnoses");

            _singularToPlural.Add("datum", "data");
            _singularToPlural.Add("medium", "media");
            _singularToPlural.Add("analysis", "analyses");

            _singularToPlural.Add("node_child", "node_children");
            _singularToPlural.Add("child", "children");

            _singularToPlural.Add("experience", "experiences");
            _singularToPlural.Add("day", "days");

            _singularToPlural.Add("comment", "comments");
            _singularToPlural.Add("foobar", "foobars");
            _singularToPlural.Add("newsletter", "newsletters");

            _singularToPlural.Add("old_news", "old_news");
            _singularToPlural.Add("news", "news");

            _singularToPlural.Add("series", "series");
            _singularToPlural.Add("species", "species");

            _singularToPlural.Add("quiz", "quizzes");

            _singularToPlural.Add("perspective", "perspectives");

            _singularToPlural.Add("ox", "oxen");
            _singularToPlural.Add("photo", "photos");
            _singularToPlural.Add("buffalo", "buffaloes");
            _singularToPlural.Add("tomato", "tomatoes");
            _singularToPlural.Add("dwarf", "dwarves");
            _singularToPlural.Add("elf", "elves");
            _singularToPlural.Add("information", "information");
            _singularToPlural.Add("equipment", "equipment");
            _singularToPlural.Add("bus", "buses");
            _singularToPlural.Add("status", "statuses");
            _singularToPlural.Add("status_code", "status_codes");
            _singularToPlural.Add("mouse", "mice");

            _singularToPlural.Add("louse", "lice");
            _singularToPlural.Add("house", "houses");
            _singularToPlural.Add("octopus", "octopi");
            _singularToPlural.Add("virus", "viri");
            _singularToPlural.Add("alias", "aliases");
            _singularToPlural.Add("portfolio", "portfolios");

            _singularToPlural.Add("vertex", "vertices");
            _singularToPlural.Add("matrix", "matrices");

            _singularToPlural.Add("axis", "axes");
            _singularToPlural.Add("testis", "testes");
            _singularToPlural.Add("crisis", "crises");

            _singularToPlural.Add("rice", "rice");
            _singularToPlural.Add("shoe", "shoes");

            _singularToPlural.Add("horse", "horses");
            _singularToPlural.Add("prize", "prizes");
            _singularToPlural.Add("edge", "edges");

            _pascalToUnderscore.Add("Product", "product");
            _pascalToUnderscore.Add("SpecialGuest", "special_guest");
            _pascalToUnderscore.Add("ApplicationController", "application_controller");
            _pascalToUnderscore.Add("Area51Controller", "area51_controller");

            _underscoreToCamel.Add("product", "product");
            _underscoreToCamel.Add("special_guest", "specialGuest");
            _underscoreToCamel.Add("application_controller", "applicationController");
            _underscoreToCamel.Add("area51_controller", "area51Controller");

            _pascalToUnderscoreWithoutReverse.Add("HTMLTidy", "html_tidy");
            _pascalToUnderscoreWithoutReverse.Add("HTMLTidyGenerator", "html_tidy_generator");
            _pascalToUnderscoreWithoutReverse.Add("FreeBSD", "free_bsd");
            _pascalToUnderscoreWithoutReverse.Add("HTML", "html");

            _underscoreToHuman.Add("employee_salary", "Employee salary");
            _underscoreToHuman.Add("employee_id", "Employee id");
            _underscoreToHuman.Add("underground", "Underground");

            _mixtureToTitleCase.Add("active_record", "Active Record");
            _mixtureToTitleCase.Add("ActiveRecord", "Active Record");
            _mixtureToTitleCase.Add("action web service", "Action Web Service");
            _mixtureToTitleCase.Add("Action Web Service", "Action Web Service");
            _mixtureToTitleCase.Add("Action web service", "Action Web Service");
            _mixtureToTitleCase.Add("actionwebservice", "Actionwebservice");
            _mixtureToTitleCase.Add("Actionwebservice", "Actionwebservice");

            _ordinalNumbers.Add("0", "0th");
            _ordinalNumbers.Add("1", "1st");
            _ordinalNumbers.Add("2", "2nd");
            _ordinalNumbers.Add("3", "3rd");
            _ordinalNumbers.Add("4", "4th");
            _ordinalNumbers.Add("5", "5th");
            _ordinalNumbers.Add("6", "6th");
            _ordinalNumbers.Add("7", "7th");
            _ordinalNumbers.Add("8", "8th");
            _ordinalNumbers.Add("9", "9th");
            _ordinalNumbers.Add("10", "10th");
            _ordinalNumbers.Add("11", "11th");
            _ordinalNumbers.Add("12", "12th");
            _ordinalNumbers.Add("13", "13th");
            _ordinalNumbers.Add("14", "14th");
            _ordinalNumbers.Add("20", "20th");
            _ordinalNumbers.Add("21", "21st");
            _ordinalNumbers.Add("22", "22nd");
            _ordinalNumbers.Add("23", "23rd");
            _ordinalNumbers.Add("24", "24th");
            _ordinalNumbers.Add("100", "100th");
            _ordinalNumbers.Add("101", "101st");
            _ordinalNumbers.Add("102", "102nd");
            _ordinalNumbers.Add("103", "103rd");
            _ordinalNumbers.Add("104", "104th");
            _ordinalNumbers.Add("110", "110th");
            _ordinalNumbers.Add("1000", "1000th");
            _ordinalNumbers.Add("1001", "1001st");

            _underscoresToDashes.Add("street", "street");
            _underscoresToDashes.Add("street_address", "street-address");
            _underscoresToDashes.Add("person_street_address", "person-street-address");
        }

        #endregion

        [Fact]
        public void PluralizePlurals()
        {
            Assert.Equal("plurals", "plurals".Pluralize());
            Assert.Equal("Plurals", "Plurals".Pluralize());
        }

        [Fact]
        public void Pluralize()
        {
            foreach (var keyValuePair in _singularToPlural)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Pluralize());
                Assert.Equal(keyValuePair.Value.Capitalize(),
                             keyValuePair.Key.Capitalize().Pluralize());
            }
        }

        [Fact]
        public void Singularize()
        {
            foreach (var keyValuePair in _singularToPlural)
            {
                Assert.Equal(keyValuePair.Key, keyValuePair.Value.Singularize());
                Assert.Equal(keyValuePair.Key.Capitalize(),
                             keyValuePair.Value.Capitalize().Singularize());
            }
        }

        [Fact]
        public void TitleCase()
        {
            foreach (var keyValuePair in _mixtureToTitleCase)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Titleize());
            }
        }

        [Fact]
        public void Pascalize()
        {
            foreach (var keyValuePair in _pascalToUnderscore)
            {
                Assert.Equal(keyValuePair.Key, keyValuePair.Value.Pascalize());
            }
        }

        [Fact]
        public void Camelize()
        {
            foreach (var keyValuePair in _underscoreToCamel)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Camelize());
            }
        }

        [Fact]
        public void Underscore()
        {
            foreach (var keyValuePair in _pascalToUnderscore)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Underscore());
            }

            foreach (var keyValuePair in _pascalToUnderscoreWithoutReverse)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Underscore());
            }

            foreach (var keyValuePair in _underscoreToCamel)
            {
                Assert.Equal(keyValuePair.Key, keyValuePair.Value.Underscore());
            }

            foreach (var keyValuePair in _underscoreToHuman)
            {
                Assert.Equal(keyValuePair.Key, keyValuePair.Value.Underscore());
            }
        }

        [Fact]
        public void Humanize()
        {
            foreach (var keyValuePair in _underscoreToHuman)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Humanize());
            }
        }

        [Fact]
        public void Ordinal()
        {
            foreach (var keyValuePair in _ordinalNumbers)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Ordinalize());
            }
        }

        [Fact]
        public void Dasherize()
        {
            foreach (var keyValuePair in _underscoresToDashes)
            {
                Assert.Equal(keyValuePair.Value, keyValuePair.Key.Dasherize());
            }
        }
    }
}
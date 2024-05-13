namespace UniversityLibrary.Test
{
    using NUnit.Framework;
    public class Tests
    {
        UniversityLibrary uni = null;
        [SetUp]
        public void Setup()
        {
            uni = new UniversityLibrary();
        }

        [Test]
        public void CostructorShouldWorkCorrectly()
        {
            Assert.AreEqual(0, uni.Catalogue.Count);
        }
        [Test]
        public void AddShouldWorkCorrectly()
        {
            TextBook book = new TextBook("Secrets", "Russel", "Money");
            uni.AddTextBookToLibrary(book);
            Assert.AreEqual(1, uni.Catalogue.Count);
        }
        [Test]
        public void LonaShouldWorkCorrectly()
        {
            TextBook book = new TextBook("Secrets", "Russel", "Money");
            TextBook book2 = new TextBook("Dot", "Russel", "Money");
            TextBook book3 = new TextBook("Com", "Russel", "Money");
            TextBook book4 = new TextBook("Expert", "Russel", "Money");
            uni.AddTextBookToLibrary(book);
            uni.AddTextBookToLibrary(book2);
            uni.AddTextBookToLibrary(book3);
            uni.AddTextBookToLibrary(book4);
            Assert.AreEqual(4, uni.Catalogue.Count);
            book.Holder = "Ico";
            string msg = uni.LoanTextBook(1, "Ico");
            Assert.AreEqual(msg, uni.LoanTextBook(1, "Ico"));
        }

        [Test]
        public void LonaShouldWorkCorrectlySecondCase()
        {
            TextBook book = new TextBook("Secrets", "Russel", "Money");
            TextBook book2 = new TextBook("Dot", "Russel", "Money");
            TextBook book3 = new TextBook("Com", "Russel", "Money");
            TextBook book4 = new TextBook("Expert", "Russel", "Money");
            uni.AddTextBookToLibrary(book);
            uni.AddTextBookToLibrary(book2);
            uni.AddTextBookToLibrary(book3);
            uni.AddTextBookToLibrary(book4);
            Assert.AreEqual(4, uni.Catalogue.Count);
            string msg = uni.LoanTextBook(1, "Ico");
            book.Holder = string.Empty;
            Assert.AreEqual(msg, uni.LoanTextBook(1, "Ico"));
        }

        [Test]
        public void ReturnBookShouldWorkCorrectly()
        {
            TextBook book = new TextBook("Secrets", "Russel", "Money");
            uni.AddTextBookToLibrary(book);
            Assert.AreEqual(1, uni.Catalogue.Count);
            string msg = uni.ReturnTextBook(1);
            TextBook book1 = new TextBook("Secrets", "Russel", "Money");
            uni.AddTextBookToLibrary(book1);
            Assert.AreEqual(msg, uni.ReturnTextBook(1));
            Assert.AreEqual(book.Holder, string.Empty);
        }
    }
}
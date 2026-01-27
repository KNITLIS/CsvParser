using CsvParser.Parser;
using CsvParser.Reader;
using CsvParser.Shaper;

namespace CsvParser
{
    public static class CsvParserBuilder
    {
        public static ISetReaderStep GetBuilder()
        {
            return new CsvPBReaderStep();
        }
    }

    internal class CsvPBReaderStep : ISetReaderStep
    {
        public ISetShaperStep SetDefaultReader(StreamReader inputStream)
        {
            return GetShaperStep(config => new DefaultReader(inputStream, config));
        }

        public ISetShaperStep SetCustomReader(Func<ReaderConfiguration, IReader> setuper)
        {
            return GetShaperStep(config => setuper(config));
        }

        private static CsvPBShaperStep GetShaperStep(ReaderCreator rc) => new(rc);
    }

    internal class CsvPBShaperStep(ReaderCreator rc) : ISetShaperStep
    {
        public IFeaturesStep<string[]> ShapeAsStringArray()
        {
            return GetShaperFeaturesStep(rc, conf => new NoShaper());
        }

        public IFeaturesStep<TShape> SetDefaultShaper<TShape>() where TShape : struct
        {
            return GetShaperFeaturesStep(rc, conf => new DefaultShaper<TShape>(conf));
        }

        public IFeaturesStep<TShape> SetCustomShaper<TShape>(Func<ShaperConfiguration, IShaper<TShape>> setuper)
            where TShape : struct
        {
            return GetShaperFeaturesStep(rc, conf => setuper(conf));
        }

        private static CsvPBFeaturesStep<TShape> GetShaperFeaturesStep<TShape>(ReaderCreator rc, ShaperCreator<TShape> sc) 
            => new(rc, sc);
    }

    internal class CsvPBFeaturesStep<TShape> : IFeaturesStep<TShape>
    {
        private ParserConfiguration parserConf = new(); 
        private ReaderConfiguration readerConf = new();
        private ShaperConfiguration shaperConf = new();
        private readonly ReaderCreator rc;
        private readonly ShaperCreator<TShape> sc;

        internal CsvPBFeaturesStep(ReaderCreator rc, ShaperCreator<TShape> sc)
        {
            this.sc = sc;
            this.rc = rc;
        }

        public IFeaturesStep<TShape> SetRecordLenegth(int length)
        {
            readerConf.RecordLength = length;
            return this;
        }

        public IFeaturesStep<TShape> SetSeparator(char separator)
        {
            readerConf.Separator = separator;
            return this;
        }

        public IFeaturesStep<TShape> SetEscaper(char escaper)
        {
            readerConf.Escaper = escaper;
            return this;
        }

        public IFeaturesStep<TShape> HasHeaders()
        {
            parserConf.HasHeaders = true;
            return this;
        }

        public IFeaturesStep<TShape> EnsureHeaders()
        {
            shaperConf.EnsureHeaders = true;
            return this;
        }

        public ICsvParser<TShape> Build()
        {
            return new CsvParser<TShape>(rc(readerConf), parserConf, sc(shaperConf));
        }
    }

    internal delegate IReader ReaderCreator(ReaderConfiguration config);
    internal delegate IShaper<TShape> ShaperCreator<TShape>(ShaperConfiguration config);

    #region Steps
    public interface ISetReaderStep
    {
        public ISetShaperStep SetDefaultReader(StreamReader inputStream);
        public ISetShaperStep SetCustomReader(Func<ReaderConfiguration, IReader> setuper);
    }

    public interface ISetShaperStep
    {
        public IFeaturesStep<string[]> ShapeAsStringArray();
        public IFeaturesStep<TShape> SetDefaultShaper<TShape>() where TShape : struct;
        public IFeaturesStep<TShape> SetCustomShaper<TShape>(Func<ShaperConfiguration, IShaper<TShape>> setuper)
            where TShape : struct;
    }

    public interface IFeaturesStep<TShape>
    {
        public IFeaturesStep<TShape> SetRecordLenegth(int length);
        public IFeaturesStep<TShape> SetSeparator(char separator);
        public IFeaturesStep<TShape> SetEscaper(char escaper);
        public IFeaturesStep<TShape> HasHeaders();
        public IFeaturesStep<TShape> EnsureHeaders();
        public ICsvParser<TShape> Build();
    }
    #endregion
}

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
            return GetShaperStep(config => new DefaultReader3(inputStream, config));
        }

        //public ISetShaperStep SetCustomReader<TReader>() where TReader : ReaderBase, new()
        //{
        //    return GetShaperStep(
        //        config => new TReader()
        //        {
        //            Configuration = config
        //        }
        //    );
        //}

        public ISetShaperStep SetCustomReader(Func<ReaderConfiguration, IReader> setuper)
        {
            return GetShaperStep(config => setuper(config));
        }

        private static CsvPBShaperStep GetShaperStep(ReaderCreator rc) => new(rc);
    }

    internal class CsvPBShaperStep(ReaderCreator rc) : ISetShaperStep
    {
        public IFeaturesStep ShapeAsStringArray()
        {
            return new CsvPBFeaturesStep(rc);
        }
        public IShaperFeaturesStep<TShape> SetDefaultShaper<TShape>() where TShape : struct //TODO: ensure headers
        {
            return GetShaperFeaturesStep(rc, conf => new DefaultShaper<TShape>(conf));
        }

        //public IShaperFeaturesStep<TShape> SetCustomShaper<TShaper, TShape>()
        //    where TShaper : ShaperBase<TShape>, new()
        //    where TShape : struct
        //{
        //    return GetShaperFeaturesStep(rc, 
        //        conf => new TShaper()
        //        {
        //            Configuration = conf
        //        }
        //    );
        //}

        public IShaperFeaturesStep<TShape> SetCustomShaper<TShape>(Func<ShaperConfiguration, IShaper<TShape>> setuper)
            where TShape : struct
        {
            return GetShaperFeaturesStep(rc, conf => setuper(conf));
        }

        private static CsvPBShapedFeaturesStep<TShape> GetShaperFeaturesStep<TShape>(ReaderCreator rc, ShaperCreator<TShape> sc) 
            where TShape : struct => new(rc, sc);
    }

    internal class CsvPBFeaturesStep(ReaderCreator rc) : IFeaturesStep
    {
        protected ReaderCreator readerCreator = rc;

        protected ReaderConfiguration readerConf = new();
        protected ParserConfiguration parserConf = new();
        public IFeaturesStep SetRecordLenegth(int length)
        {
            readerConf.RecordLength = length;
            return this;
        }
        public IFeaturesStep SetSeparator(char separator)
        {
            readerConf.Separator = separator;
            return this;
        }
        public IFeaturesStep SetEscaper(char escaper)
        {
            readerConf.Escaper = escaper;
            return this;
        }
        public IFeaturesStep HasHeaders()
        {
            parserConf.HasHeaders = true;
            return this;
        }

        public ICsvParser<string[]> Build()
        {
            return new CsvUntypedParser(readerCreator(readerConf), parserConf); 
        }
    }

    internal class CsvPBShapedFeaturesStep<TShape> : CsvPBFeaturesStep, IShaperFeaturesStep<TShape>
        where TShape : struct
    {
        private ShaperConfiguration shaperConf = new();
        private readonly ShaperCreator<TShape> sc;

        internal CsvPBShapedFeaturesStep(ReaderCreator rc, ShaperCreator<TShape> sc)
            : base(rc)
        {
            this.sc = sc;
            parserConf.HasHeaders = true;
        }

        IShaperFeaturesStep<TShape> IShaperFeaturesStep<TShape>.SetRecordLenegth(int length)
        {
            base.SetRecordLenegth(length);
            return this;
        }

        IShaperFeaturesStep<TShape> IShaperFeaturesStep<TShape>.SetSeparator(char separator)
        {
            base.SetSeparator(separator);
            return this;
        }

        IShaperFeaturesStep<TShape> IShaperFeaturesStep<TShape>.SetEscaper(char escaper)
        {
            base.SetEscaper(escaper);
            return this;
        }

        IShaperFeaturesStep<TShape> IShaperFeaturesStep<TShape>.HasNoHeaders()
        {
            parserConf.HasHeaders = false;
            return this;
        }

        public IShaperFeaturesStep<TShape> EnsureHeaders()
        {
            shaperConf.EnsureHeaders = true;
            return this;
        }

        public new ICsvParser<TShape> Build()
        {
            return new CsvTypedParser<TShape>(readerCreator(readerConf), parserConf, sc(shaperConf));
        }
    }

    internal delegate IReader ReaderCreator(ReaderConfiguration config);
    internal delegate IShaper<TShape> ShaperCreator<TShape>(ShaperConfiguration config);

    #region Steps
    public interface ISetReaderStep
    {
        public ISetShaperStep SetDefaultReader(StreamReader inputStream);
        //public ISetShaperStep SetCustomReader<TReader>() where TReader : ReaderBase, new();
        public ISetShaperStep SetCustomReader(Func<ReaderConfiguration, IReader> setuper);
    }

    public interface ISetShaperStep
    {
        public IFeaturesStep ShapeAsStringArray();
        public IShaperFeaturesStep<TShape> SetDefaultShaper<TShape>() where TShape : struct;
        //public IShaperFeaturesStep<TShape> SetCustomShaper<TShaper, TShape>() 
        //    where TShaper : ShaperBase<TShape>, new()
        //    where TShape : struct;
        public IShaperFeaturesStep<TShape> SetCustomShaper<TShape>(Func<ShaperConfiguration, IShaper<TShape>> setuper)
            where TShape : struct;
    }

    public interface IFeaturesStep
    {
        public IFeaturesStep SetRecordLenegth(int length);
        public IFeaturesStep SetSeparator(char separator);
        public IFeaturesStep SetEscaper(char escaper);
        public IFeaturesStep HasHeaders();
        public ICsvParser<string[]> Build();
    }

    public interface IShaperFeaturesStep<TShape>
    {
        public IShaperFeaturesStep<TShape> SetRecordLenegth(int length);
        public IShaperFeaturesStep<TShape> SetSeparator(char separator);
        public IShaperFeaturesStep<TShape> SetEscaper(char escaper);

        [Obsolete($"Without headers struct properties order is not guaranteed, use {nameof(CsvPBShaperStep.ShapeAsStringArray)} instead")]
        public IShaperFeaturesStep<TShape> HasNoHeaders();
        public IShaperFeaturesStep<TShape> EnsureHeaders();
        public ICsvParser<TShape> Build();
    }
    #endregion
}

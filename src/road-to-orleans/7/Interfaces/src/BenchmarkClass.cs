using MemoryPack;
using MessagePack;
using Orleans;

namespace Interfaces;

[Immutable]
[MemoryPackable]
public partial class IntClass1
{

    #region Constants & Statics

    public static IntClass1 Create()
    {
        var result = new IntClass1();
        result.Initialize();
        return result;
    }

    #endregion

    #region Properties

    [MemoryPackOrder(0)]
    public int MyProperty1 { get; set; }

    [MemoryPackOrder(1)]
    public int MyProperty2 { get; set; }

    [MemoryPackOrder(2)]
    public int MyProperty3 { get; set; }

    [MemoryPackOrder(3)]
    public int MyProperty4 { get; set; }

    [MemoryPackOrder(4)]
    public int MyProperty5 { get; set; }

    [MemoryPackOrder(5)]
    public int MyProperty6 { get; set; }

    [MemoryPackOrder(6)]
    public int MyProperty7 { get; set; }

    [MemoryPackOrder(7)]
    public int MyProperty8 { get; set; }

    [MemoryPackOrder(8)]
    public int MyProperty9 { get; set; }

    #endregion

    #region Methods

    public void Initialize()
    {
        MyProperty1 = MyProperty2 =
            MyProperty3 = MyProperty4 = MyProperty5 = MyProperty6 = MyProperty7 = MyProperty8 = MyProperty9 = 10;
    }

    #endregion

}

[Immutable]
[MessagePackObject]
public partial class IntClass2
{

    #region Constants & Statics

    public static IntClass2 Create()
    {
        var result = new IntClass2();
        result.Initialize();
        return result;
    }

    #endregion

    #region Properties

    [Key(0)]
    public int MyProperty1 { get; set; }

    [Key(1)]
    public int MyProperty2 { get; set; }

    [Key(2)]
    public int MyProperty3 { get; set; }

    [Key(3)]
    public int MyProperty4 { get; set; }

    [Key(4)]
    public int MyProperty5 { get; set; }

    [Key(5)]
    public int MyProperty6 { get; set; }

    [Key(6)]
    public int MyProperty7 { get; set; }

    [Key(7)]
    public int MyProperty8 { get; set; }

    [Key(8)]
    public int MyProperty9 { get; set; }

    #endregion

    #region Methods

    public void Initialize()
    {
        MyProperty1 = MyProperty2 =
            MyProperty3 = MyProperty4 = MyProperty5 = MyProperty6 = MyProperty7 = MyProperty8 = MyProperty9 = 10;
    }

    #endregion

}

[Immutable]
[GenerateSerializer]
[Alias("Interfaces.IntClass3")]
public partial class IntClass3
{

    #region Constants & Statics

    public static IntClass3 Create()
    {
        var result = new IntClass3();
        result.Initialize();
        return result;
    }

    #endregion

    #region Properties

    [Id(0)]
    public int MyProperty1 { get; set; }

    [Id(1)]
    public int MyProperty2 { get; set; }

    [Id(2)]
    public int MyProperty3 { get; set; }

    [Id(3)]
    public int MyProperty4 { get; set; }

    [Id(4)]
    public int MyProperty5 { get; set; }

    [Id(5)]
    public int MyProperty6 { get; set; }

    [Id(6)]
    public int MyProperty7 { get; set; }

    [Id(7)]
    public int MyProperty8 { get; set; }

    [Id(8)]
    public int MyProperty9 { get; set; }

    #endregion

    #region Methods

    public void Initialize()
    {
        MyProperty1 = MyProperty2 =
            MyProperty3 = MyProperty4 = MyProperty5 = MyProperty6 = MyProperty7 = MyProperty8 = MyProperty9 = 10;
    }

    #endregion

}

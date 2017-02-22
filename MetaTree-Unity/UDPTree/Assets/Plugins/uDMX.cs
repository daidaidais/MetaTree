//
// uDMX APIラッパ
//
// 使用するためには、uDMX.dllが必要です。
//
// Copyright(C) 2015 Team Hashilus.
//

using System;
using System.Runtime.InteropServices;


/// <summary>
/// uDMX APIのラッパークラス
/// </summary>
public static class uDMX {
	/// <summary>
	/// チャンネル番号の最小値
	/// </summary>
	public const Int32 MIN_CHANNEL = 1;

	/// <summary>
	/// チャンネル番号の最大値
	/// </summary>
	/// <remarks>
	/// デフォルトは512であるが、uDMXの設定ダイアログで変更できる。
	/// </remarks>
	public const Int32 MAX_CHANNEL = 512;

	/// <summary>
	/// チャンネルにセットできる値の最大値
	/// </summary>
	public const Int32 MAX_VALUE = 255;

	/// <summary>
	/// uDMXの設定ダイアログを表示する
	/// </summary>
	[DllImport("uDMX")]
	public static extern Boolean Configure();
	
	/// <summary>
	/// uDMX経由でデバイスが接続されているかどうかを調べる
	/// </summary>
	[DllImport("uDMX")]
	public static extern Boolean Connected();
	
	/// <summary>
	/// DMX機器のチャンネルに値をセットする
	/// </summary>
	/// <param name="channel">設定するチャンネル番号（1～512）</param>
	/// <param name="value">チャンネルにセットする値（0～255）</param>
	[DllImport("uDMX")]
	public static extern Boolean ChannelSet(Int32 channel, Int32 value);
	
	/// <summary>
	/// DMX機器の複数チャンネルに同時に値をセットする
	/// </summary>
	/// <param name="channelCnt">設定するチャンネル数</param>
	/// <param name="channel">設定を開始するチャンネル番号（1～512）</param>
	/// <param name="value">チャンネルにセットする値（0～255）の配列（channelCntと同じサイズ）</param>
	[DllImport("uDMX")]
	public static extern Boolean ChannelsSet(Int32 channelCnt, Int32 channel, Int32[] value);
}



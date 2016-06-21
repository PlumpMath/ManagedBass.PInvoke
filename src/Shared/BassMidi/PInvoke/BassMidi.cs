﻿using System;
using System.Runtime.InteropServices;

namespace ManagedBass.Midi
{
    /// <summary>
    /// Wraps BassMidi: bassmidi.dll
    /// 
    /// <para>Supports: .midi, .mid, .rmi, .kar</para>
    /// </summary>
    public static partial class BassMidi
    {
        const int BassMidiFontEx = 0x1000000;

        /// <summary>
        /// Chorus Mix Channel.
        /// </summary>
        public const int ChorusChannel = -1;

        /// <summary>
        /// Reverb Mix Channel.
        /// </summary>
        public const int ReverbChannel = -2;

        /// <summary>
        /// User FX Channel.
        /// </summary>
        public const int UserFXChannel = -3;

        /// <summary>
        /// Creates a sample stream to render real-time MIDI events.
        /// </summary>
        /// <param name="Channels">The number of MIDI channels: 1 (min) - 128 (max).</param>
        /// <param name="Flags">A combination of <see cref="BassFlags"/>.</param>
        /// <param name="Frequency">Sample rate (in Hz) to render/play the MIDI at (0 = the rate specified in the <see cref="Bass.Init" /> call; 1 = the device's current output rate or the <see cref="Bass.Init"/> rate if that is not available).</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// This function creates a stream solely for real-time MIDI events.
        /// As it's not based on any file, the stream has no predetermined length and is never-ending.
        /// Seeking isn't possible, but it is possible to reset everything, including playback buffer, by calling <see cref="Bass.ChannelPlay" /> (Restart = <see langword="true" />) or <see cref="Bass.ChannelSetPosition" /> (Position = 0).
        /// </para>
        /// <para>
        /// MIDI events are applied using the <see cref="StreamEvent(int,int,MidiEventType,int)" /> function.
        /// If the stream is being played (it's not a decoding channel), then there will be some delay in the effect of the events being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength" /> and <see cref="Bass.UpdatePeriod" /> options.
        /// </para>
        /// <para>
        /// If a stream has 16 MIDI channels, then channel 10 defaults to percussion/drums and the rest melodic, otherwise they are all melodic.
        /// That can be changed using <see cref="StreamEvent(int,int,MidiEventType,int)" /> and <see cref="MidiEventType.Drums"/>.
        /// </para>
        /// <para>
        /// Soundfonts provide the sounds that are used to render a MIDI stream.
        /// A default soundfont configuration is applied initially to the new MIDI stream, which can subsequently be overriden using <see cref="StreamSetFonts(int,MidiFont[],int)" />.
        /// </para>
        /// <para>To play a MIDI file, use <see cref="CreateStream(string,long,long,BassFlags,int)" />.</para>
        /// <para><b>Platform-specific</b></para>
        /// <para>
        /// On Android and iOS, sinc interpolation requires a NEON-supporting CPU; the <see cref="BassFlags.SincInterpolation"/> flag will otherwise be ignored.
        /// Sinc interpolation is not available on Windows CE.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Init"><see cref="Bass.Init" /> has not been successfully called.</exception>
        /// <exception cref="Errors.NotAvailable">Only decoding channels (<see cref="BassFlags.Decode"/>) are allowed when using the <see cref="Bass.NoSoundDevice"/>. The <see cref="BassFlags.AutoFree"/> flag is also unavailable to decoding channels.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Channels" /> is not valid.</exception>
        /// <exception cref="Errors.SampleFormat">The sample format is not supported by the device/drivers. If the stream is more than stereo or the <see cref="BassFlags.Float"/> flag is used, it could be that they are not supported.</exception>
        /// <exception cref="Errors.Speaker">The specified Speaker Flags are invalid. The device/drivers do not support them, they are attempting to assign a stereo stream to a mono speaker or 3D functionality is enabled.</exception>
        /// <exception cref="Errors.Memory">There is insufficient memory.</exception>
        /// <exception cref="Errors.No3D">Could not initialize 3D support.</exception>
        /// <exception cref="Errors.Unknown">Some other mystery problem!</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamCreate")]
        public static extern int CreateStream(int Channels, BassFlags Flags = BassFlags.Default, int Frequency = 0);

        /// <summary>
        /// Creates a sample stream from a sequence of MIDI events.
        /// </summary>
        /// <param name="Events">An array of <see cref="MidiEvent" />s containing the event sequence to play (the array should be terminated with a <see cref="MidiEventType.End"/> event).</param>
        /// <param name="PulsesPerQuarterNote">The number of pulses per quarter note (or ticks per beat) value of the MIDI stream to create.</param>
        /// <param name="Flags">A combination of <see cref="BassFlags"/>.</param>
        /// <param name="Frequency">Sample rate to render/play the MIDI at (0 = the rate specified in the <see cref="Bass.Init" /> call; 1 = the device's current output rate (or the <see cref="Bass.Init"/> BASS_Init rate if that is not available).</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// This function creates a 16 channel MIDI stream to play a predefined sequence of MIDI events.
        /// Any of the standard MIDI events can be used, but <see cref="MidiEventType.Level"/>, <see cref="MidiEventType.Transpose"/>, and <see cref="MidiEventType.SystemEx"/> events are not available and will be ignored.
        /// The sequence should end with a <see cref="MidiEventType.End"/> event.
        /// Multiple tracks are possible via the <see cref="MidiEventType.EndTrack"/> event, which signals the end of a track; the next event will be in a new track.
        /// Any <see cref="MidiEventType.Tempo"/> events should be in the first track.
        /// </para>
        /// <para>The event sequence is copied, so the events array does not need to persist beyond the function call.</para>
        /// <para>
        /// Soundfonts provide the sounds that are used to render a MIDI stream.
        /// A default soundfont configuration is applied initially to the new MIDI stream, which can subsequently be overridden using <see cref="StreamSetFonts(int,MidiFont[],int)" />.
        /// </para>
        /// <para><b>Platform-specific</b></para>
        /// <para>
        /// On Android and iOS, sinc interpolation requires a NEON-supporting CPU; the <see cref="BassFlags.SincInterpolation"/> flag will otherwise be ignored.
        /// Sinc interpolation is not available on Windows CE.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Init"><see cref="Bass.Init" /> has not been successfully called.</exception>
        /// <exception cref="Errors.NotAvailable">Only decoding channels (<see cref="BassFlags.Decode"/>) are allowed when using the <see cref="Bass.NoSoundDevice"/>. The <see cref="BassFlags.AutoFree"/> flag is also unavailable to decoding channels.</exception>
        /// <exception cref="Errors.NoInternet">No internet connection could be opened. Can be caused by a bad <see cref="Bass.NetProxy"/> setting.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Events" /> are not valid.</exception>
        /// <exception cref="Errors.Timeout">The server did not respond to the request within the timeout period, as set with the <see cref="Bass.NetTimeOut"/> config option.</exception>
        /// <exception cref="Errors.FileOpen">The file could not be opened.</exception>
        /// <exception cref="Errors.FileFormat">The file's format is not recognised/supported.</exception>
        /// <exception cref="Errors.SampleFormat">The sample format is not supported by the device/drivers. If the stream is more than stereo or the <see cref="BassFlags.Float"/> flag is used, it could be that they are not supported.</exception>
        /// <exception cref="Errors.Speaker">The specified Speaker Flags are invalid. The device/drivers do not support them, they are attempting to assign a stereo stream to a mono speaker or 3D functionality is enabled.</exception>
        /// <exception cref="Errors.Memory">There is insufficient memory.</exception>
        /// <exception cref="Errors.No3D">Could not initialize 3D support.</exception>
        /// <exception cref="Errors.Unknown">Some other mystery problem!</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamCreateEvents")]
        public static extern int CreateStream(MidiEvent[] Events, int PulsesPerQuarterNote, BassFlags Flags = BassFlags.Default, int Frequency = 0);

        /// <summary>
        /// Applies an event to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the event to (as returned by <see cref="CreateStream(int,BassFlags,int)" />).</param>
        /// <param name="Channel">The MIDI channel to apply the event to... 0 = channel 1.</param>
        /// <param name="Event">The event to apply (see <see cref="MidiEventType" /> for details).</param>
        /// <param name="Parameter">The event parameter (see <see cref="MidiEventType" /> for details).</param>
        /// <returns>If successful, <see langword="true" /> is returned, else <see langword="false" /> is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>Apart from the "global" events, all events apply only to the specified MIDI channel.</para>
        /// <para>
        /// Except for the "non-MIDI" events, events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels"/> attribute.
        /// </para>
        /// <para>
        /// Event syncs (see <see cref="SyncFlags" />) are not triggered by this function.
        /// If sync triggering is wanted, <see cref="StreamEvents(int,MidiEventsMode,MidiEvent[],int)" /> can be used instead.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.Parameter">One of the other parameters is invalid.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamEvent")]
        public static extern bool StreamEvent(int Handle, int Channel, MidiEventType Event, int Parameter);

        /// <summary>
        /// Applies an event to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the event to (as returned by <see cref="CreateStream(int,BassFlags,int)" />).</param>
        /// <param name="Channel">The MIDI channel to apply the event to... 0 = channel 1.</param>
        /// <param name="Event">The event to apply (see <see cref="MidiEventType" /> for details).</param>
        /// <param name="LowParameter">The event parameter (LOBYTE), (see <see cref="MidiEventType" /> for details).</param>
        /// <param name="HighParameter">The event parameter (HIBYTE), (see <see cref="MidiEventType" /> for details).</param>
        /// <returns>If successful, <see langword="true" /> is returned, else <see langword="false" /> is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>Apart from the "global" events, all events apply only to the specified MIDI channel.</para>
        /// <para>
        /// Except for the "non-MIDI" events, events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels"/> attribute.
        /// </para>
        /// <para>
        /// Event syncs (see <see cref="SyncFlags" />) are not triggered by this function.
        /// If sync triggering is wanted, <see cref="StreamEvents(int,MidiEventsMode,MidiEvent[],int)" /> can be used instead.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.Parameter">One of the other parameters is invalid.</exception>
        public static bool StreamEvent(int Handle, int Channel, MidiEventType Event, byte LowParameter, byte HighParameter)
        {
            return StreamEvent(Handle, Channel, Event, BitHelper.MakeLong(LowParameter, HighParameter));
        }

        #region StreamEvents
        /// <summary>
        /// Applies any number of events to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the events to.</param>
        /// <param name="Mode">Midi Events Mode.</param>
        /// <param name="Events">The event data (raw data - byte[]).</param>
        /// <param name="Length">Length of Events data according to <paramref name="Mode"/>.</param>
        /// <returns>If successful, the number of events processed is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// Events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels" /> attribute.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle"/> is not valid.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Mode"/> is not valid.</exception>
        [DllImport(DllName, EntryPoint= "BASS_MIDI_StreamEvents")]
        public static extern int StreamEvents(int Handle, MidiEventsMode Mode, IntPtr Events, int Length);

        [DllImport(DllName)]
        static extern int BASS_MIDI_StreamEvents(int Handle, MidiEventsMode Mode, MidiEvent[] Events, int Length);

        [DllImport(DllName)]
        static extern int BASS_MIDI_StreamEvents(int Handle, MidiEventsMode Mode, byte[] Events, int Length);

        /// <summary>
        /// Applies any number of events to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the events to.</param>
        /// <param name="Mode">Midi Events Mode.</param>
        /// <param name="Events">The event data (an array of <see cref="MidiEvent" /> structures).</param>
        /// <param name="Length">No of <see cref="MidiEvent"/> items... 0 = No of items in <paramref name="Events"/> array.</param>
        /// <returns>If successful, the number of events processed is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// Events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels" /> attribute.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle"/> is not valid.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Mode"/> is not valid.</exception>
        public static int StreamEvents(int Handle, MidiEventsMode Mode, MidiEvent[] Events, int Length = 0)
        {
            return BASS_MIDI_StreamEvents(Handle, Mode & ~MidiEventsMode.Raw, Events, Length == 0 ? Events.Length : Length);
        }

        /// <summary>
        /// Applies any number of events to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the events to.</param>
        /// <param name="Mode">Midi Events Mode.</param>
        /// <param name="Raw">The event data (raw data - byte[]).</param>
        /// <param name="Length">No of <see cref="byte"/>s... 0 = No of items in <paramref name="Raw"/> array.</param>
        /// <returns>If successful, the number of events processed is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// Events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels" /> attribute.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle"/> is not valid.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Mode"/> is not valid.</exception>
        public static int StreamEvents(int Handle, MidiEventsMode Mode, byte[] Raw, int Length = 0)
        {
            return BASS_MIDI_StreamEvents(Handle, MidiEventsMode.Raw | Mode, Raw, Length == 0 ? Raw.Length : Length);
        }

        /// <summary>
        /// Applies any number of events to a MIDI stream.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the events to.</param>
        /// <param name="Mode">Midi Events Mode.</param>
        /// <param name="Channel">To overcome the 16 channel limit, the event data's channel information can optionally be overridden by adding the new channel number to this parameter, where 1 = the 1st channel - else leave to 0.</param>
        /// <param name="Raw">The event data (raw data - byte[]).</param>
        /// <param name="Length">No of <see cref="byte"/>s... 0 = No of items in <paramref name="Raw"/> array.</param>
        /// <returns>If successful, the number of events processed is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// Events applied to a MIDI file stream can subsequently be overridden by events in the file itself, and will also be overridden when seeking or looping.
        /// That can be avoided by using additional channels, allocated via the <see cref="ChannelAttribute.MidiChannels" /> attribute.
        /// </para>
        /// <para>
        /// If the MIDI stream is being played (it's not a decoding channel), then there will be some delay in the effect of the event being heard. 
        /// This latency can be reduced by making use of the <see cref="Bass.PlaybackBufferLength"/> and <see cref="Bass.UpdatePeriod"/> config options when creating the stream.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle"/> is not valid.</exception>
        /// <exception cref="Errors.Parameter"><paramref name="Mode"/> is not valid.</exception>
        public static int StreamEvents(int Handle, MidiEventsMode Mode, int Channel, byte[] Raw, int Length = 0)
        {
            return BASS_MIDI_StreamEvents(Handle, MidiEventsMode.Raw + Channel | Mode, Raw, Length == 0 ? Raw.Length : Length);
        }
        #endregion

        /// <summary>
        /// Gets a HSTREAM handle for a MIDI channel (e.g. to set DSP/FX on individual MIDI channels).
        /// </summary>
        /// <param name="Handle">The midi stream to get a channel from.</param>
        /// <param name="Channel">The MIDI channel... 0 = channel 1. Or one of the following special channels:
        /// <para><see cref="ChorusChannel"/> = Chorus mix channel. The default chorus processing is replaced by the stream's processing.</para>
        /// <para><see cref="ReverbChannel"/> = Reverb mix channel. The default reverb processing is replaced by the stream's processing.</para>
        /// <para><see cref="UserFXChannel"/> = User effect mix channel.</para>
        /// </param>
        /// <returns>If successful, the channel handle is returned, else 0 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// <para>
        /// By default, MIDI channels do not have streams assigned to them;
        /// a MIDI channel only gets a stream when this function is called, which it then keeps until the MIDI stream is freed. 
        /// MIDI channel streams can also be freed before then via <see cref="Bass.StreamFree" />.
        /// Each MIDI channel stream increases the CPU usage slightly, even if there are no DSP/FX set on them, so for optimal performance they should not be activated when unnecessary.
        /// </para>
        /// <para>
        /// The MIDI channel streams have a different path to the final mix than the BASSMIDI reverb/chorus processing, which means that the reverb/chorus will not be present in the data received by any DSP/FX set on the streams and nor will the reverb/chorus be applied to the DSP/FX output; 
        /// the reverb/chorus processing will use the channel's original data.
        /// </para>
        /// <para>
        /// The MIDI channel streams can only be used to set DSP/FX on the channels. 
        /// They cannot be used with <see cref="Bass.ChannelGetData(int,IntPtr,int)" /> or <see cref="Bass.ChannelGetLevel(int)" /> to visualise the channels, for example, 
        /// but that could be achieved via a DSP function instead.
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.NotAvailable"><paramref name="Channel" /> is not valid.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetChannel")]
        public static extern int StreamGetChannel(int Handle, int Channel);

        /// <summary>
        /// Retrieves the current value of an event in a MIDI stream channel.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the event from (as returned by <see cref="CreateStream(int,BassFlags,int)"/>.</param>
        /// <param name="Channel">The MIDI channel to get the event value from... 0 = channel 1.</param>
        /// <param name="Event">
        /// The event value to retrieve.
        /// With the drum key events (<see cref="MidiEventType.DrumCutOff"/>/etc) and the <see cref="MidiEventType.Note"/> and <see cref="MidiEventType.ScaleTuning"/> events, the HIWORD can be used to specify which key/note to get the value from.</param>
        /// <returns>The event parameter if successful - else -1 (use <see cref="Bass.LastError" /> to get the error code).</returns>
        /// <remarks>SYNCs can be used to be informed of when event values change.</remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.Parameter">One of the other parameters is invalid.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetEvent")]
        public static extern int StreamGetEvent(int Handle, int Channel, MidiEventType Event);

        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetEvents")]
        public static extern int StreamGetEvents(int handle, int track, int filter, [In, Out] MidiEvent[] events);

        public static MidiEvent[] StreamGetEvents(int handle, int track, int filter)
        {
            var count = StreamGetEvents(handle, track, filter, null);

            if (count <= 0)
                return null;

            var events = new MidiEvent[count];

            StreamGetEvents(handle, track, filter, events);

            return events;
        }

        /// <summary>
        /// Retrieves the soundfont configuration of a MIDI stream, or the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <param name="Fonts">An <see cref="IntPtr"/> to retrieve the soundfont configuration.</param>
        /// <param name="Count">The maximum number of elements to retrieve. This and fonts can be 0, to get the number of elements in the soundfont configuration.</param>
        /// <returns>If successful, the number of soundfonts in the configuration (which can be higher than count) is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>When a soundfont matching the MIDI file is loaded, it will be the first element in the returned configuration.</remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetFonts")]
        public static extern int StreamGetFonts(int Handle, IntPtr Fonts, int Count);

        /// <summary>
        /// Retrieves the soundfont configuration of a MIDI stream, or the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <param name="Fonts">An array to retrieve the soundfont configuration.</param>
        /// <param name="Count">The maximum number of elements to retrieve in the fonts array. This and fonts can be 0, to get the number of elements in the soundfont configuration.</param>
        /// <returns>If successful, the number of soundfonts in the configuration (which can be higher than count) is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>When a soundfont matching the MIDI file is loaded, it will be the first element in the returned configuration.</remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetFonts")]
        public static extern int StreamGetFonts(int Handle, [In][Out] MidiFont[] Fonts, int Count);

        /// <summary>
        /// Retrieves the soundfont configuration of a MIDI stream, or the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <returns>An array of <see cref="MidiFont" /> configuration entries if successfull - or <see langword="null" /> on error.</returns>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        public static MidiFont[] StreamGetFonts(int Handle)
        {
            var count = StreamGetFontsCount(Handle);

            if (count <= 0)
                return null;

            var arr = new MidiFont[count];
            StreamGetFonts(Handle, arr, count);
            return arr;
        }

        [DllImport(DllName)]
        static extern int BASS_MIDI_StreamGetFonts(int handle, [In][Out] MidiFontEx[] fonts, int count);

        /// <summary>
        /// Retrieves the soundfont configuration of a MIDI stream, or the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <param name="Fonts">An array to retrieve the soundfont configuration.</param>
        /// <param name="Count">The maximum number of elements to retrieve in the fonts array. This and fonts can be 0, to get the number of elements in the soundfont configuration.</param>
        /// <returns>If successful, the number of soundfonts in the configuration (which can be higher than count) is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>When a soundfont matching the MIDI file is loaded, it will be the first element in the returned configuration.</remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        public static int StreamGetFonts(int Handle, MidiFontEx[] Fonts, int Count)
        {
            return BASS_MIDI_StreamGetFonts(Handle, Fonts, Count | BassMidiFontEx);
        }

        /// <summary>
        /// Retrieves the soundfont configuration of a MIDI stream, or the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <returns>An array of <see cref="MidiFontEx" /> configuration entries if successfull - or <see langword="null" /> on error.</returns>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        public static MidiFontEx[] StreamGetFontsEx(int Handle)
        {
            var count = StreamGetFontsCount(Handle);

            if (count <= 0)
                return null;

            var arr = new MidiFontEx[count];
            StreamGetFonts(Handle, arr, count);
            return arr;
        }

        /// <summary>
        /// Retrieves the number of elements in the soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to retrieve the soundfont configuration of... 0 = get default soundfont configuration.</param>
        /// <returns>If successful, the number of soundfonts in the configuration is returned, else -1 is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        public static int StreamGetFontsCount(int Handle) => StreamGetFonts(Handle, IntPtr.Zero, 0);

        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetMark")]
        public static extern bool StreamGetMark(int handle, MidiMarkerType type, int index, out MidiMarker mark);

        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamGetMarks")]
        public static extern int StreamGetMarks(int handle, int track, MidiMarkerType type, [In, Out] MidiMarker[] marks);

        public static MidiMarker[] StreamGetMarks(int handle)
        {
            var markCount = StreamGetMarks(handle, -1, MidiMarkerType.Marker, null);

            if (markCount <= 0)
                return null;

            var marks = new MidiMarker[markCount];
            StreamGetMarks(handle, -1, MidiMarkerType.Marker, marks);

            return marks;
        }
        
		/// <summary>
		/// Preloads the samples required by a MIDI file stream.
		/// </summary>
		/// <param name="Handle">The MIDI stream handle.</param>
		/// <returns>If successful, <see langword="true" /> is returned, else <see langword="false" /> is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
		/// <remarks>
		/// <para>
        /// Samples are normally loaded as they are needed while rendering a MIDI stream, which can result in CPU spikes, particularly with packed soundfonts.
        /// That generally won't cause any problems, but when smooth/constant performance is critical this function can be used to preload the samples before rendering, so avoiding the need to load them while rendering.
        /// </para>
		/// <para>Preloaded samples can be compacted/unloaded just like any other samples, so it is probably wise to disable the <see cref="Compact"/> option when preloading samples, to avoid any chance of the samples subsequently being automatically unloaded.</para>
		/// <para>This function should not be used while the MIDI stream is being rendered, as that could interrupt the rendering.</para>
		/// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.NotAvailable">The stream is for real-time events only, so it's not possible to know what presets are going to be used. Use <see cref="FontLoad" /> instead.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamLoadSamples")]
        public static extern bool StreamLoadSamples(int Handle);

        /// <summary>
        /// Applies a soundfont configuration to a MIDI stream, or sets the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the soundfonts to... 0 = set default soundfont configuration.</param>
        /// <param name="Fonts">An array of <see cref="MidiFont" /> soundfonts to apply.</param>
        /// <param name="Count">The number of elements in the fonts array.</param>
        /// <returns>If successful, <see langword="true" /> is returned, else <see langword="false" /> is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// Multiple soundfonts can be stacked, each providing different presets, for example.
        /// When a preset is present in multiple soundfonts, the earlier soundfont in the array has priority.
        /// When a soundfont matching the MIDI file is loaded, that remains loaded when calling this function, and has priority over all other soundfonts.
        /// When a preset is not available on a non-0 bank in any soundfont, BASSMIDI will try to fall back to bank 0; first the LSB and then the MSB if still unsuccessful.
        /// <para>
        /// Changing the default configuration only affects subsequently created MIDI streams.
        /// Existing streams that are using the previous default configuration will continue to use that previous configuration.
        /// </para>
        /// <para>On Windows, the default default configuration will be to use the Creative 4MB (CT4MGM.SF2) or 2MB (CT2MGM.SF2) soundfont when present in the Windows system directory.</para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.Parameter">Something in the <paramref name="Fonts" /> array is invalid, check the soundfont handles.</exception>
        [DllImport(DllName, EntryPoint = "BASS_MIDI_StreamSetFonts")]
        public static extern int StreamSetFonts(int Handle, MidiFont[] Fonts, int Count);

        [DllImport(DllName)]
        static extern int BASS_MIDI_StreamSetFonts(int handle, MidiFontEx[] fonts, int count);

        /// <summary>
        /// Applies a soundfont configuration to a MIDI stream, or sets the default soundfont configuration.
        /// </summary>
        /// <param name="Handle">The MIDI stream to apply the soundfonts to... 0 = set default soundfont configuration.</param>
        /// <param name="Fonts">An array of <see cref="MidiFontEx" /> soundfonts to apply.</param>
        /// <param name="Count">The number of elements in the fonts array.</param>
        /// <returns>If successful, <see langword="true" /> is returned, else <see langword="false" /> is returned. Use <see cref="Bass.LastError" /> to get the error code.</returns>
        /// <remarks>
        /// Multiple soundfonts can be stacked, each providing different presets, for example.
        /// When a preset is present in multiple soundfonts, the earlier soundfont in the array has priority.
        /// When a soundfont matching the MIDI file is loaded, that remains loaded when calling this function, and has priority over all other soundfonts.
        /// When a preset is not available on a non-0 bank in any soundfont, BASSMIDI will try to fall back to bank 0; first the LSB and then the MSB if still unsuccessful.
        /// <para>
        /// Changing the default configuration only affects subsequently created MIDI streams.
        /// Existing streams that are using the previous default configuration will continue to use that previous configuration.
        /// </para>
        /// <para>On Windows, the default default configuration will be to use the Creative 4MB (CT4MGM.SF2) or 2MB (CT2MGM.SF2) soundfont when present in the Windows system directory.</para>
        /// </remarks>
        /// <exception cref="Errors.Handle"><paramref name="Handle" /> is not valid.</exception>
        /// <exception cref="Errors.Parameter">Something in the <paramref name="Fonts" /> array is invalid, check the soundfont handles.</exception>
        public static int StreamSetFonts(int Handle, MidiFontEx[] Fonts, int Count)
        {
            return BASS_MIDI_StreamSetFonts(Handle, Fonts, Count | BassMidiFontEx);
        }
    }
}
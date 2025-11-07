using PubSea.Framework.Exceptions;
using PubSea.Framework.Services.Abstractions;
using Sqids;

namespace PubSea.Framework.Services.Implementations;

internal sealed class HashIdService : IHashIdService
{
    private const string FAKE_HASHID_PREFIX = "1111111111";

    private readonly SqidsEncoder<long> _encoder;

    public HashIdService(HashIdOptions options)
    {
        var sqidsOptions = new SqidsOptions
        {
            MinLength = options.MinHashLength,
            Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789",
        };
        _encoder = new SqidsEncoder<long>(sqidsOptions);
    }

    string IHashIdService.Encode(int value)
    {
        return _encoder.Encode(value);
    }

    int IHashIdService.Decode(string value)
    {
        var number = ((IHashIdService)this).DecodeLong(value);
        return (int)number;
    }

    string IHashIdService.EncodeLong(long value)
    {
        return _encoder.Encode(value);
    }

    long IHashIdService.DecodeLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new SeaException();
        }

        var decodedValues = _encoder.Decode(value);
        if (decodedValues is not [var firstNumber] ||
            !IsCanonical(value, firstNumber))
        {
            throw new SeaException();
        }

        return firstNumber;
    }

    string IHashIdService.GenerateFakeHashId()
    {
        var randomNumber = string.Join(string.Empty, FAKE_HASHID_PREFIX, Random.Shared.Next(0, 9999));
        return ((IHashIdService)this).EncodeLong(long.Parse(randomNumber));
    }

    bool IHashIdService.IsFakeHashId(string? hashId)
    {
        if (string.IsNullOrWhiteSpace(hashId))
        {
            return true;
        }

        var decodedNumber = ((IHashIdService)this).DecodeLong(hashId).ToString();

        if (decodedNumber.StartsWith(FAKE_HASHID_PREFIX))
        {
            return true;
        }

        return false;
    }

    private bool IsCanonical(string value, long number)
    {
        return string.Equals(_encoder.Encode(number), value);
    }
}

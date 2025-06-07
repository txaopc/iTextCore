This is a repo forked from the iText Core community.

[agpl]: LICENSE.md
[building]: BUILDING.md
[contributing]: CONTRIBUTING.md
[itext]: https://itextpdf.com/
[github]: https://github.com/itext/itext7-dotnet
[latest]: https://github.com/itext/itext7-dotnet/releases/latest
[nuget]: https://www.nuget.org/packages/itext7
[sales]: https://itextpdf.com/sales
[gratis]: https://en.wikipedia.org/wiki/Gratis_versus_libre
[rups]: https://github.com/itext/i7j-rups
[pdfhtml]: https://github.com/itext/i7n-pdfhtml
[pdfsweep]: https://github.com/itext/i7n-pdfsweep

In Vietnam's government PDF e-documents, digital signatures include embedded PNG signature images. This is why I'm customizing iText 7.2.3 to handle this implementation (Newer versions (of iText) appear to disallow PNG usage, likely due to PDF/A format compliance requirements.)

And, I have modified the library to support digital signing and signature verification using the RSASSA-PSS algorithm scheme.

The workflow is implemented through these stages:

Step 1: Add RSASSA-PSS scheme OID to the itext.sign module in SecureIDs.cs

```C#
public class SecurityIDs {
    ...
    //Add OID for RSASSA-PSS schema
    public const String ID_RSASSA_PSS = "1.2.840.113549.1.1.10";
}
```

Step 2: Add RSASSA-PSS algorithm name to the algorithmNames dictionary in the EncryptionAlgorithms class:

```C#
static EncryptionAlgorithms() {
    ...
    // Add RSA Signature Scheme with Appendix - Probabilistic Signature (RSASSA-PSS) name
    algorithmNames.Put("1.2.840.113549.1.1.10", "RSAandMGF1");
}
```

Step 3: PdfPKCS7 Class Updates:

- Enhance SetExternalDigest to handle RSASSA-PSS:

```C#
public virtual void SetExternalDigest(byte[] digest, byte[] rsaData, String digestEncryptionAlgorithm) {
    externalDigest = digest;
    externalRsaData = rsaData;
    if (digestEncryptionAlgorithm != null) {
        if (digestEncryptionAlgorithm.Equals("RSA")) {
            this.digestEncryptionAlgorithmOid = SecurityIDs.ID_RSA;
        }
        else {
            if (digestEncryptionAlgorithm.Equals("DSA")) {
                this.digestEncryptionAlgorithmOid = SecurityIDs.ID_DSA;
            }
            else {
                if (digestEncryptionAlgorithm.Equals("ECDSA")) {
                    this.digestEncryptionAlgorithmOid = SecurityIDs.ID_ECDSA;
                }
                else {
                    if (digestEncryptionAlgorithm.ToUpper().EndsWith("RSAANDMGF1"))
                    {
                        this.digestEncryptionAlgorithmOid = SecurityIDs.ID_RSASSA_PSS;
                    }
                    else
                    {
                        throw new PdfException(SignExceptionMessageConstant.UNKNOWN_KEY_ALGORITHM).SetMessageParams(digestEncryptionAlgorithm
                            );
                    }
                }
            }
        }
    }
}
```

- Modify GetEncodedPKCS7 to accept PSS parameters:

```C#
 public virtual byte[] GetEncodedPKCS7(byte[] secondDigest, PdfSigner.CryptoStandard sigtype, ITSAClient tsaClient
     , ICollection<byte[]> ocsp, ICollection<byte[]> crlBytes) {
    ...
    // Add the digestEncryptionAlgorithm
    v = new Asn1EncodableVector();
    v.Add(new DerObjectIdentifier(digestEncryptionAlgorithmOid));
    //Add parameters for RSASSA-PSS schema
    if(digestEncryptionAlgorithmOid == SecurityIDs.ID_RSASSA_PSS)
    {
        Asn1Encodable rsaPssParameters = Org.BouncyCastle.Security.SignerUtilities.GetDefaultX509Parameters(GetDigestAlgorithm());
        v.Add(rsaPssParameters);
    }
    else 
    { 
        v.Add(Org.BouncyCastle.Asn1.DerNull.Instance); 
    }
    signerinfo.Add(new DerSequence(v));
    ...
}


```

If this repository is helpful to you, consider supporting me with a coffee.

<a href="https://buymeacoffee.com/txaopc" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" width="170px"></a>

or

[<img src="https://github.com/user-attachments/assets/4f103c82-7938-4865-927f-a6deab3f29cd" height=200/>](https://github.com/user-attachments/assets/4f103c82-7938-4865-927f-a6deab3f29cd)

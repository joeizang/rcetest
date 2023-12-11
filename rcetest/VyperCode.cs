namespace rcetest;

public static class VyperCode
{
    public const string BlindAuction = @"

struct Bid:
  blindedBid: bytes32
  deposit: uint256

MAX_BIDS: constant(int128) = 128

event AuctionEnded:
    highestBidder: address
    highestBid: uint256

beneficiary: public(address)
biddingEnd: public(uint256)
revealEnd: public(uint256)

ended: public(bool)

highestBid: public(uint256)
highestBidder: public(address)

bids: HashMap[address, Bid[128]]
bidCounts: HashMap[address, int128]

pendingReturns: HashMap[address, uint256]


@external
def __init__(_beneficiary: address, _biddingTime: uint256, _revealTime: uint256):
    self.beneficiary = _beneficiary
    self.biddingEnd = block.timestamp + _biddingTime
    self.revealEnd = self.biddingEnd + _revealTime


@external
@payable
def bid(_blindedBid: bytes32):
    # Check if bidding period is still open
    assert block.timestamp < self.biddingEnd

    # Check that payer hasn't already placed maximum number of bids
    numBids: int128 = self.bidCounts[msg.sender]
    assert numBids < MAX_BIDS

    # Add bid to mapping of all bids
    self.bids[msg.sender][numBids] = Bid({
        blindedBid: _blindedBid,
        deposit: msg.value
        })
    self.bidCounts[msg.sender] += 1


@internal
def placeBid(bidder: address, _value: uint256) -> bool:
    if (_value <= self.highestBid):
        return False

    if (self.highestBidder != empty(address)):
        self.pendingReturns[self.highestBidder] += self.highestBid

    self.highestBid = _value
    self.highestBidder = bidder

    return True


@external
def reveal(_numBids: int128, _values: uint256[128], _fakes: bool[128], _secrets: bytes32[128]):
    # Check that bidding period is over
    assert block.timestamp > self.biddingEnd

    # Check that reveal end has not passed
    assert block.timestamp < self.revealEnd

    # Check that number of bids being revealed matches log for sender
    assert _numBids == self.bidCounts[msg.sender]

    # Calculate refund for sender
    refund: uint256 = 0
    for i in range(MAX_BIDS):
        # Note that loop may break sooner than 128 iterations if i >= _numBids
        if (i >= _numBids):
            break

        # Get bid to check
        bidToCheck: Bid = (self.bids[msg.sender])[i]

        # Check against encoded packet
        value: uint256 = _values[i]
        fake: bool = _fakes[i]
        secret: bytes32 = _secrets[i]
        blindedBid: bytes32 = keccak256(concat(
            convert(value, bytes32),
            convert(fake, bytes32),
            secret
        ))

        # Bid was not actually revealed
        # Do not refund deposit
        assert blindedBid == bidToCheck.blindedBid

        # Add deposit to refund if bid was indeed revealed
        refund += bidToCheck.deposit
        if (not fake and bidToCheck.deposit >= value):
            if (self.placeBid(msg.sender, value)):
                refund -= value

        # Make it impossible for the sender to re-claim the same deposit
        zeroBytes32: bytes32 = empty(bytes32)
        bidToCheck.blindedBid = zeroBytes32

    # Send refund if non-zero
    if (refund != 0):
        send(msg.sender, refund)


# Withdraw a bid that was overbid.
@external
def withdraw():
    pendingAmount: uint256 = self.pendingReturns[msg.sender]
    if (pendingAmount > 0):
        self.pendingReturns[msg.sender] = 0

        # Then send return
        send(msg.sender, pendingAmount)


# End the auction and send the highest bid to the beneficiary.
@external
def auctionEnd():
    # Check that reveal end has passed
    assert block.timestamp > self.revealEnd

    # Check that auction has not already been marked as ended
    assert not self.ended

    # Log auction ending and set flag
    log AuctionEnded(self.highestBidder, self.highestBid)
    self.ended = True

    # Transfer funds to beneficiary
    send(self.beneficiary, self.highestBid)
";
}
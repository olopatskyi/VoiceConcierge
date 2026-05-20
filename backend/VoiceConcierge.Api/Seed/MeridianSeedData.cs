namespace VoiceConcierge.Api.Seed;

/// <summary>
/// Initial FAQ content sourced from the Meridian Casino & Resort PRD.
/// Inserted by <see cref="SeedRunner"/> only when the FAQ table is empty.
/// </summary>
public static class MeridianSeedData
{
    public static readonly IReadOnlyList<SeedFaqItem> Faqs = new SeedFaqItem[]
    {
        // ─── General ───────────────────────────────────────────────────────
        new("What are your casino hours?",
            "Our casino is open 24 hours a day, 7 days a week.",
            "gaming"),

        new("Where are you located?",
            "We are located on the Las Vegas Strip in Nevada.",
            "general"),

        new("What time is check-in?",
            "Check-in begins at 4:00 PM. Early check-in is available subject to room availability.",
            "accommodations"),

        new("What time is check-out?",
            "Standard check-out is 11:00 AM. Late checkout is available for suite guests.",
            "accommodations"),

        new("Is parking free?",
            "Self-parking is complimentary for all guests. Valet is complimentary for hotel guests and twenty-five dollars for visitors.",
            "general"),

        new("Do you have valet parking?",
            "Yes — valet parking is complimentary for hotel guests and twenty-five dollars for visitors.",
            "general"),

        new("What is the dress code?",
            "Smart casual throughout the property. Formal attire is required at Aurelia restaurant and at Eclipse Lounge after 8 PM.",
            "general"),

        new("Is there an age requirement to gamble?",
            "Guests must be 21 or older to enter the casino floor or any bar. The hotel and family restaurants welcome all ages.",
            "gaming"),

        new("Do you have Wi-Fi?",
            "Complimentary Wi-Fi is available throughout the property. Premium high-speed Wi-Fi is included in all suites.",
            "general"),

        new("How do I reach the front desk?",
            "You can reach our front desk by dialing extension 0 from any in-room phone.",
            "general"),

        // ─── Casino Gaming ─────────────────────────────────────────────────
        new("Is the poker room open right now?",
            "Yes — our poker room operates 24 hours a day, 7 days a week. We offer Texas Hold'em, Omaha, and Seven Card Stud.",
            "gaming"),

        new("What poker games do you offer?",
            "Texas Hold'em (No Limit and Limit), Omaha, and Seven Card Stud. We also run daily tournaments at 11 AM and 7 PM with a two hundred dollar buy-in.",
            "gaming"),

        new("When are the poker tournaments?",
            "Daily tournaments run at 11 AM and 7 PM, with a two hundred dollar buy-in.",
            "gaming"),

        new("How many slot machines do you have?",
            "Over two thousand machines with bets ranging from one cent to one thousand dollars per spin. Latest video slots, classic reels, and progressive jackpots are all available.",
            "gaming"),

        new("What is the largest slot jackpot?",
            "The largest progressive jackpot is currently at four point two million dollars.",
            "gaming"),

        new("How many blackjack tables do you have?",
            "Forty blackjack tables with minimums from twenty-five dollars up to ten thousand. We offer single deck, six-deck, and Spanish 21.",
            "gaming"),

        new("What is the blackjack minimum bet?",
            "Minimums start at twenty-five dollars and go up to ten thousand dollars depending on the table.",
            "gaming"),

        new("Do you have roulette?",
            "Yes — twelve roulette tables including American, European, and French roulette. Minimums start at fifteen dollars.",
            "gaming"),

        new("Do you have baccarat?",
            "Eight baccarat tables in the main casino, plus a private high-limit salon with four additional tables. Minimums start at fifty dollars and high-limit from five thousand.",
            "gaming"),

        new("Do you offer craps?",
            "Yes — six craps tables with minimums from fifteen dollars. Complimentary lessons are available daily at 10 AM.",
            "gaming"),

        new("Can I learn how to play craps?",
            "Absolutely — complimentary lessons are available daily at 10 AM at the craps tables.",
            "gaming"),

        new("Tell me about the sports book.",
            "Our sports book is an eighty-seat theater with a forty-foot screen and full bar service. Mobile betting is available throughout the property via the Meridian app.",
            "gaming"),

        new("Can I bet on sports from my room?",
            "Yes — mobile sports betting is available throughout the property through the Meridian app.",
            "gaming"),

        new("What is the High Limit Salon?",
            "A private gaming area with dedicated hosts, available by invitation or with a fifty-thousand-dollar credit line. It includes private restrooms, complimentary dining, and personal butler service.",
            "gaming"),

        new("Do you have a rewards program?",
            "Yes — Meridian Rewards. Earn points on all play. Tiers are Gold, Platinum, Diamond, and invitation-only Black.",
            "gaming"),

        // ─── Accommodations ────────────────────────────────────────────────
        new("Tell me about your room options.",
            "We offer Deluxe Rooms from $299, Premier Rooms from $449, Luxury Suites from $799, Penthouse Suites from $2,500, and the Chairman's Villa by inquiry.",
            "accommodations"),

        new("How big is a Deluxe Room?",
            "Four hundred and fifty square feet with a king or two queens, and a city or pool view. From two hundred and ninety-nine dollars a night.",
            "accommodations"),

        new("Tell me about Premier Rooms.",
            "Five hundred and fifty square feet, king bed with a sitting area, and a Strip view. From four hundred and forty-nine dollars a night.",
            "accommodations"),

        new("What does a Luxury Suite include?",
            "Nine hundred square feet with a separate living room and a marble bathroom with a soaking tub. From seven hundred and ninety-nine dollars a night.",
            "accommodations"),

        new("What is the Penthouse Suite?",
            "Eighteen hundred square feet with two bedrooms, a dining room, butler's pantry, and a private terrace. From two thousand five hundred dollars a night.",
            "accommodations"),

        new("Tell me about the Chairman's Villa.",
            "Forty-five hundred square feet with three bedrooms, a private pool, optional personal chef, and 24-hour butler service. Available by inquiry only.",
            "accommodations"),

        new("Do you have accessible rooms?",
            "Yes — accessible rooms are available in every category, with roll-in showers, lowered amenities, and visual alerts. Please request at the time of booking.",
            "accommodations"),

        // ─── Dining ────────────────────────────────────────────────────────
        new("What is your best restaurant?",
            "Our signature restaurant is Aurelia, featuring modern French cuisine by Chef Marcus Webb and holding two Michelin stars. The tasting menu is two hundred and eighty-five dollars per person.",
            "dining"),

        new("Tell me about Aurelia.",
            "Modern French fine dining by Chef Marcus Webb, with two Michelin stars. Open for dinner only, 6 to 10 PM. Reservations required, formal attire.",
            "dining"),

        new("What time does Aurelia open?",
            "Aurelia serves dinner only, from 6 PM to 10 PM. Reservations are required.",
            "dining"),

        new("Is there a dress code at Aurelia?",
            "Yes — formal attire is required at Aurelia.",
            "dining"),

        new("Tell me about Silk Road.",
            "Pan-Asian cuisine with a sushi bar, robata grill, and dim sum. Open 11 AM to 11 PM daily, smart casual. Forty to eighty dollars per person.",
            "dining"),

        new("Where can I get sushi?",
            "Silk Road has a full sushi bar alongside robata grill and dim sum. Open 11 AM to 11 PM daily.",
            "dining"),

        new("Tell me about The Steakhouse.",
            "Classic American steakhouse with dry-aged beef and fresh seafood. Open 5 PM to 11 PM, smart casual. Seventy to one hundred fifty dollars per person.",
            "dining"),

        new("Where can I get breakfast?",
            "Café Meridian serves a breakfast buffet daily from 7 to 11 AM for forty-five dollars. Lunch and dinner menu continues until midnight.",
            "dining"),

        new("Tell me about Café Meridian.",
            "Casual all-day dining. Breakfast buffet 7 to 11 AM at forty-five dollars, lunch and dinner menu until midnight. No reservations needed.",
            "dining"),

        new("Do I need reservations for the restaurants?",
            "Aurelia requires reservations. Silk Road and The Steakhouse recommend them, especially for dinner. Café Meridian and the Pool Bar do not.",
            "dining"),

        new("Can I eat by the pool?",
            "Yes — the Pool Bar & Grill serves burgers, salads, and cocktails from 10 AM to 6 PM seasonally, for hotel guests only.",
            "dining"),

        // ─── Bars & Lounges ────────────────────────────────────────────────
        new("Tell me about Eclipse Lounge.",
            "Our rooftop bar with Strip views, craft cocktails, and a champagne menu. Open 5 PM to 2 AM, formal attire after 8 PM.",
            "bars"),

        new("Where can I get a great whiskey?",
            "The Vault is our whiskey and cigar lounge, with over four hundred whiskey selections and a humidor of premium cigars. Open 4 PM to 2 AM, 21 and up.",
            "bars"),

        new("Are drinks free on the casino floor?",
            "Complimentary drinks are offered to active players at any of our six casino floor bars.",
            "bars"),

        // ─── Amenities ─────────────────────────────────────────────────────
        new("Do you have a spa?",
            "Yes — Meridian Spa offers massage, facials, body treatments, and couples suites. Open 8 AM to 8 PM. Booking 24 hours in advance is recommended.",
            "amenities"),

        new("Is the fitness center open 24 hours?",
            "Yes — the fitness center is open 24 hours for hotel guests. We offer personal training, Peloton bikes, free weights, and a yoga studio.",
            "amenities"),

        new("Do you have a pool?",
            "Three pools, including an adults-only infinity pool. Open 8 AM to 8 PM. Cabanas are available from three hundred dollars a day with a one hundred dollar food credit.",
            "amenities"),

        new("How much are pool cabanas?",
            "Cabanas start at three hundred dollars a day and include a one hundred dollar food credit.",
            "amenities"),

        new("Is there a theater?",
            "The Meridian Theater is a twelve-hundred-seat venue with internationally acclaimed artists in residency. Tickets start at ninety-five dollars, with VIP packages available.",
            "amenities"),

        new("Do you have a nightclub?",
            "NOVA is our premier nightclub, open Friday and Saturday from 10:30 PM to 4 AM. Cover is fifty dollars general, tables from two thousand. Hotel guests can request the guest list.",
            "amenities"),

        new("When is the nightclub open?",
            "NOVA is open Friday and Saturday nights from 10:30 PM to 4 AM.",
            "amenities"),

        // ─── Celebrations ──────────────────────────────────────────────────
        new("Can you host a wedding?",
            "Yes — we offer venues from an intimate chapel to the grand ballroom. Packages range from five thousand to one hundred fifty thousand dollars and include a dedicated wedding coordinator.",
            "celebrations"),

        new("Do you host private events?",
            "Yes — meeting rooms and ballrooms from 500 to 15,000 square feet, with full catering and AV services. Corporate rates are available.",
            "celebrations"),

        new("Do you have a birthday package?",
            "Our celebration package starts at five hundred dollars and includes a room upgrade, champagne, custom cake, and dinner credit. We ask for 72 hours' notice.",
            "celebrations"),

        new("I want to propose to my partner. Can you help?",
            "Our celebration package starts at five hundred dollars with a room upgrade, champagne, and dinner credit. For something elaborate, Eclipse Lounge offers private terrace reservations with stunning Strip views. Our concierge desk can orchestrate the perfect moment.",
            "celebrations"),

        new("Do you have bachelor or bachelorette packages?",
            "Yes — VIP packages start at three thousand dollars and include a suite, nightclub table, pool cabana, and spa credits. Please book at least one week in advance.",
            "celebrations"),

        // ─── Partner Discounts ─────────────────────────────────────────────
        new("Any good restaurants nearby?",
            "Carbone is about five minutes away and offers our guests fifteen percent off plus priority reservations with your room key. We also have three excellent restaurants on property.",
            "partners"),

        new("Do I get a discount at Carbone?",
            "Yes — Meridian guests receive fifteen percent off the food bill and priority reservations at Carbone. Just show your room key.",
            "partners"),

        new("Are there nearby attractions?",
            "Omega Mart is a popular immersive art experience and our guests get ten dollars off admission. We can also arrange helicopter tours, Top Golf, or transportation to Fremont Street.",
            "partners"),

        new("Can I do a helicopter tour?",
            "Yes — Vegas Nights Aviation offers helicopter tours of the Strip and our guests receive twenty percent off plus complimentary champagne.",
            "partners"),

        new("Do you partner with Top Golf?",
            "Yes — Meridian guests get a free hour with any two-hour booking at Top Golf Las Vegas.",
            "partners"),

        new("Where can I go if your spa is fully booked?",
            "If Meridian Spa is fully booked, Spa Aquae at the JW Marriott offers our guests ten percent off treatments.",
            "partners"),

        new("How do I get to Fremont Street?",
            "We can arrange transportation, and Meridian guests receive a VIP SlotZilla pass to skip the line at the Fremont Street Experience.",
            "partners"),
    };
}

public record SeedFaqItem(string Question, string Answer, string Category);
